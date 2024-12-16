using Microsoft.Extensions.Hosting;

namespace Quickie.Configuration.Idempotency;

/// <summary>
/// Background service to remove expired idempotency key.
/// </summary>
/// <param name="idempotencyProvider"></param>
public class IdempotencyBackgroundService(IIdempotencyProvider idempotencyProvider) : BackgroundService
{
    private readonly QuickieOptions _options = GlobalQuickieConfigData.Options;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromHours(_options.IdempotencyConfiguration?.RunBackgroundServiceEveryHour ?? 1), stoppingToken);
            await idempotencyProvider.RemoveExpiredKeys();
        }
    }
}