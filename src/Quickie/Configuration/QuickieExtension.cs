using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Quickie.Configuration.Middleware;
using Quickie.DataHandlers.Crud.Implementation;

namespace Quickie.Configuration;

/// <summary>
/// Quickie extensions
/// </summary>
public static class QuickieExtension
{
    /// <summary>
    /// Configures the application to use Quickie services and middleware based on provided options.
    /// </summary>
    /// <param name="app">Application request pipeline builder.</param>
    /// <returns>IApplicationBuilder</returns>
    public static IApplicationBuilder AddQuickie(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        var options = GlobalQuickieConfigData.Options; 
        ArgumentNullException.ThrowIfNull(options);
        
        if (options.IdempotencyConfiguration is { Enable: true })
        {
            app.UseMiddleware<IdempotencyMiddleware>();
        }

        if (!options.RateLimitingConfiguration?.DisableRateLimiting ?? true)
        {
            app.UseRateLimiter();
        }
        return app;
    }
    
    /// <summary>
    /// Registers Quickie services in the DI container, including all custom services.
    /// </summary>
    /// <param name="services">DI register and resolver.</param>
    /// <param name="configureOptions">Delegate to configure quickie option.</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection QuickieConfig(this IServiceCollection services, Action<QuickieOptions>? configureOptions = null)
    {
        var options = new QuickieOptions();
        configureOptions?.Invoke(options);
        GlobalQuickieConfigData.Initialize(options);
        
        // manual di registration for custom idempotency provider
        if (options.IdempotencyConfiguration?.Enable ?? false)
        { 
            if (options.IdempotencyConfiguration.Provider is null or InMemoryIdempotencyProvider)
            {
                options.IdempotencyConfiguration.Provider = new InMemoryIdempotencyProvider();
                services.AddSingleton<IIdempotencyProvider, InMemoryIdempotencyProvider>();
            }
            else
            { 
                ArgumentNullException.ThrowIfNull(options.IdempotencyConfiguration.Provider);
                services.AddSingleton<IIdempotencyProvider>(options.IdempotencyConfiguration.Provider);
            } 
            services.AddHostedService<IdempotencyBackgroundService>();
        }

        // rate limiting logic
        if (!options.RateLimitingConfiguration?.DisableRateLimiting ?? true)
        { 
            options.RateLimitingConfiguration ??= new RateLimitConfiguration();
            
            services.AddRateLimiter(rlOpt =>
            {
                rlOpt.AddPolicy(options.RateLimitingConfiguration!.PolicyName!, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = options.RateLimitingConfiguration.PermitLimit!.Value,
                            Window = TimeSpan.FromSeconds(options.RateLimitingConfiguration.FromSeconds!.Value)
                        }));

                // reject status code set to 429 instead of default i.e. 503 
                rlOpt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
        }
        services.AddDataHandlersDependency();

        return services;
    }
}