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
* Custom error message is enabled. API client will see custom generic message instead of the exception messages. 

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

##### View [doc](https://sushantpt.github.io/Quickie/api/Quickie.Configuration.Idempotency.IdempotentConfiguration.html?q=IdempotentConfiguration)

## Default Idempotency provider
Quickie uses in-memory option to provide idempotency. 
#### Example:
```csharp
builder.Services.QuickieConfig(options => {
    options.IdempotencyConfiguration = new IdempotentConfiguration {
        Enable = true
    };
});
``` 
By default idempotency is disabled, above configuration enables it. Now, every POST request requires `X-Idempotency-Key` header. Header's value will be saved in-mem. Every 1 hour background service will reset this in-memory pool. You can customize idempotency lifespan and interval period to run background service. 
#### Example:
```csharp
builder.Services.QuickieConfig(options => {
    options.IdempotencyConfiguration = new IdempotentConfiguration {
        Enable = true,
        RunBackgroundServiceEveryHour = 2,
        IdempotencyLifespan = TimeSpan.FromHours(5)
    };
});
``` 
Above config will run background service every 2 hours and sets idempotency key's lifespan to 5 hours. It means each key's lifespan is 5 hours and any key whose lifespan has completed will be removed by backgroud service from in-memory pool in every 2 hours. 

## Custom Idempotency provider
Instead of Quickie's default idempotency provider, you can have your own custom provider using Redis, MongoDB, SQL databases, or any other storage solution. Implement your provider with `IIdempotencyProvider`. Here is an example using [Redis](https://redis.io/docs/latest/develop/clients/dotnet/):

#### Run Redis server
```bash
docker run -d --name redis -p 6379:6379 redis
```
##### Check process status

```bash
docker ps
```

**You are good to go if Redis is running.**

```csharp
using Quickie.Configuration.Idempotency;
using StackExchange.Redis;

namespace sample.idempotent.redis.Configuration;

public class RedisIdempotencyProvider : IIdempotencyProvider
{
    private readonly IDatabase _redisDatabase;
    public RedisIdempotencyProvider(IConnectionMultiplexer redisConnection)
    {
        _redisDatabase = redisConnection.GetDatabase();
    }

    public async ValueTask<bool> ExistsAsync(string key)
    {
        var check = await _redisDatabase.KeyExistsAsync(key);
        return check;
    }

    public async ValueTask MarkAsync(string key)
    {
        var lifespan = TimeSpan.FromHours(1); // holds this key only for 1 hr
        await _redisDatabase.StringSetAsync(key, DateTime.UtcNow.ToString("o"), lifespan);
    }

    public async ValueTask RemoveExpiredKeys()
    {
        // redis automatically removes keys when their lifespan.
        await ValueTask.CompletedTask;
    }
}
```

Now, configuration look something like this:
```csharp
builder.Services.QuickieConfig(options => {
    options.IdempotencyConfiguration = new IdempotentConfiguration {
        Enable = true,
        RunBackgroundServiceEveryHour = 1,
        Provider = new RedisIdempotencyProvider(redisConnection)
    };
});
```

The background service will run once every hour to clean up expired idempotency keys from Redis. Although Redis automatically removes expired keys, the background service can be used for additional tasks like manually cleaning up or performing other maintenance if needed.

> `IdempotencyLifespan` is only for quickie's default idempotency provider. If you're using a custom provider like redis or other options, you'll need to implement the lifespan logic for the keys yourself as demonstrated in the example above. `RedisIdempotencyProvider` will be automatically resolved by Quickie as well.

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

> Internally, Quickie implements rate limiting using the [Fixed Window algorithm](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-9.0#slide), which tracks requests based on IP addresses. This allows you to limit the number of requests a user can make within a specific time window. For example, the configuration in the code above sets a limit on the number of requests that can be made from a given IP address within a defined time window. If the limit is exceeded, Quickie will return a 429 Too Many Requests status code. [src](https://sushantpt.github.io/Quickie/api/Quickie.Configuration.QuickieExtension.html)

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
If database related occur while creating an entity, instead of showing actual exception message, it will show: `Data not created.`

* Show actual error
```csharp
builder.Services.QuickieConfig(options => {
    options.ShowCustomErrorMessage = false;
});
```
Above configuration will show actual exception message for all sorta errors. 
#### Example:
If database related occur while creating an entity, it will show: `The specified key 'user_id' was not ...`


Configuration src doc [here](../api/Quickie.Configuration.html)