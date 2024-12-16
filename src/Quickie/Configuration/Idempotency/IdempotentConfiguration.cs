using System.ComponentModel.DataAnnotations;

namespace Quickie.Configuration.Idempotency;

public class IdempotentConfiguration
{
    /// <summary>
    /// Enable idempotency. By default, idempotency is <c>false</c>
    /// </summary>
    public bool Enable { get; set; } = false;
    
    /// <summary>
    /// Your custom idempotency provider
    /// </summary>
    public IIdempotencyProvider? Provider { get; set; }

    /// <summary>
    /// Lifespan of keys. By default, its 1 hour
    /// </summary>
    public TimeSpan IdempotencyLifespan { get; set; } = TimeSpan.FromHours(1);
    
    /// <summary>
    /// Run background service every x hour? By default, 1 hour
    /// </summary>
    [Range(minimum: 0, maximum: 24)]
    public int RunBackgroundServiceEveryHour { get; set; } = 1;
}