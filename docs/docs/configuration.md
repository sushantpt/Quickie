# Configuration

Quickie offers flexible configuration options to customize its behavior according to your application needs. This guide covers all available configuration options and their usage.

## Basic Setup

To configure Quickie in your application, you need to add it to both your service collection and middleware pipeline:

```csharp
// In Program.cs
builder.Services.QuickieConfig();

// In middleware pipeline
app.AddQuickie();
```
Above configuration will enable default options. Default options include:
* Rate limiting
    * Accept 1 request (`PermitLimit`) every 6 seconds (`FromSeconds`) or simply understanding: allow only 10 API request every 1 minute.
    * Policy name as Quickie-Rl-Policy.
* Idempotency is disabled.
* Custom error message is enabled. API client will see custom generic message instead of the real error message. 

## Idempotency Configuration
Idempotency prevents duplicate API request. You can make your POST call idempotent. To enable idempotency:
```csharp
builder.Services.QuickieConfig(options => {
    options.IdempotencyConfiguration = new IdempotentConfiguration {
        Enable = true,                                    // enable idempotency support
        IdempotencyLifespan = TimeSpan.FromHours(2),     // set key lifespan (default: 1 hour)
        RunBackgroundServiceEveryHour = 2,               // cleanup interval (default: 1 hour)
        Provider = new CustomIdempotencyProvider()        // optional: custom provider
    };
});
```
| Property                           | Type                 | Default                     | Description |
|------------------------------------|----------------------|-----------------------------|-------------|
| `Enable`                       | `bool`                   | `false`                       | Enables/disables idempotency support |
| `Provider`                     | `IIdempotencyProvider`   | `InMemoryIdempotencyProvider` | Custom provider for idempotency handling. By default, its handle by Quickie using in-memory configuration. [more](https://sushantpt.github.io/Quickie/api/Quickie.Configuration.Idempotency.IIdempotencyProvider.html)  |
| `IdempotencyLifespan`          | `TimeSpan`                | 1 hour                      | Duration for which idempotency keys remain valid |
| `RunBackgroundServiceEveryHour`| `int`                     | 1                           | Interval (in hours) for cleanup service (0-24) |

[doc](https://sushantpt.github.io/Quickie/api/Quickie.Configuration.Idempotency.IdempotentConfiguration.html?q=IdempotentConfiguration)

## Custom Idempotency provider
Instead of Quickie's default idempotency provider, you can have your own custom provider using Redis, MongoDB, SQL databases, or any other storage solution. Implement your provider with `IIdempotencyProvider`. Here is an example using [Redis](https://redis.io/docs/latest/develop/clients/dotnet/):
```csharp
public class RedisIdempotencyProvider : IIdempotencyProvider
{
    private readonly IConnectionMultiplexer _redis;
    private readonly TimeSpan _keyLifespan;

    public RedisIdempotencyProvider(IConnectionMultiplexer redis, TimeSpan keyLifespan)
    {
        _redis = redis;
        _keyLifespan = keyLifespan;
    }

    public async ValueTask<bool> ExistsAsync(string key)
    {
        var db = _redis.GetDatabase();
        return await db.KeyExistsAsync(key);
    }

    public async ValueTask MarkAsync(string key)
    {
        var db = _redis.GetDatabase();
        await db.StringSetAsync(key, "1", _keyLifespan);
    }

    public async ValueTask RemoveExpiredKeys()
    {
        await ValueTask.CompletedTask;
    }
}
```
Now, configuration look something like this:
```csharp
builder.Services.QuickieConfig(options => {
    options.IdempotencyConfiguration = new IdempotentConfiguration {
        Enable = true,
        IdempotencyLifespan = TimeSpan.FromHours(2),
        RunBackgroundServiceEveryHour = 2,
        Provider = new RedisIdempotencyProvider()
    };
});
```

## Rate Limiting Configuration
Quickie provides rate limiting options based on IP address by default. To customize:
* If you want to disable:
```csharp
builder.Services.QuickieConfig(options => {
    options.RateLimitingConfiguration = new RateLimitConfiguration {
        DisableRateLimiting = true // this disables rate limiting
    };
});
```

#### Customize:
* Scenario: Allow 100 request in duration of 60 seconds.
```csharp
builder.Services.QuickieConfig(options => {
    options.RateLimitingConfiguration = new RateLimitConfiguration {
        PermitLimit = 100,
        FromSeconds = 60
    };
});
```
| Property              | Type   | Default             | Description |
|----------             |------  |---------            |-------------|
| `DisableRateLimiting` | bool   | `false`             | Disables/enables rate limiting |
| `PolicyName`          | string | "Quickie-Rl-Policy" | Name of the rate limit policy. This **cannot** be changed. |
| `PermitLimit`         | int    | 1                   | Number of allowed requests per window |
| `FromSeconds`         | int    | 6                   | Time window duration in seconds |

> Internally, Quickie uses the sliding door algorithm with IP addresses to check for rate limiting. [src](https://sushantpt.github.io/Quickie/api/Quickie.Configuration.QuickieExtension.html)

## Error Message Configuration
Configure how you want to show your error messages: show actual error or generic message.

* Show generic message
```csharp
builder.Services.QuickieConfig(options => {
    options.ShowCustomErrorMessage = true;
});
```
Above configuration will show custom generic error message for all sorta errors. 
#### Example:
If database related occur while creating an entity, instead of showing actual error, it will show: `Data not created.`

* Show actual error
```csharp
builder.Services.QuickieConfig(options => {
    options.ShowCustomErrorMessage = false;
});
```
Above configuration will show actual error message for all sorta errors. 
#### Example:
If database related occur while creating an entity, it will show: `The specified key 'user_id' was not ...`


Configuration src doc [here](../api/Quickie.Configuration.html)