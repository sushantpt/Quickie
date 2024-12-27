# Getting Started

To start using Quickie in your .NET project, follow these steps:

## Adding Quickie to Your Project

1. Install Quickie via NuGet:
   ```bash
   dotnet add package Quickie
   ```

2. Add Quickie configuration in your `Program.cs` file:


### Default Configuration:
```csharp
builder.Services.QuickieConfig();
app.AddQuickie();
```


### Customized Configuration
```csharp
builder.Services.QuickieConfig(options => {
    options.ShowCustomErrorMessage = false;
    options.RateLimitingConfiguration = new RateLimitConfiguration
    {
        DisableRateLimiting = false
    };
    options.IdempotencyConfiguration = new IdempotentConfiguration
    {
        Enable = true
    };
});

app.AddQuickie();
```
Simple demo it is, but you can go nuts with configuration. More [here](../docs/configuration.html)

## Explanation of Key Features
- **Rate Limiting**: Manage API usage with customizable rate-limiting policies. To know more click [here](https://en.wikipedia.org/wiki/Rate_limiting). 
- **Idempotency**: Ensure consistent results for repeated API calls, with support for custom providers. To know more click [here](https://developer.mozilla.org/en-US/docs/Glossary/Idempotent).

