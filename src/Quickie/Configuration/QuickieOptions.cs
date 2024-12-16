namespace Quickie.Configuration;

/// <summary>
/// Quickie options.
/// </summary>
public class QuickieOptions
{
    /// <summary>
    /// Idempotency configuration
    /// </summary>
    public IdempotentConfiguration? IdempotencyConfiguration { get; set; }

    /// <summary>
    /// Rate limit configuration
    /// </summary>
    public RateLimitConfiguration? RateLimitingConfiguration { get; set; }
    
    /// <summary>
    /// Instead of exception message, <c>ResponseObj</c> will be returning custom message.
    /// </summary>
    public bool ShowCustomErrorMessage { get; set; } = true;
}