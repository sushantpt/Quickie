namespace Quickie.Configuration.Options;

/// <summary>
/// Rate limit configuration. (ip based)
/// </summary>
public class RateLimitConfiguration
{
    /// <summary>
    /// Disable rate limiting. By default, rate limiting is <c>true</c>.
    /// </summary>
    public bool DisableRateLimiting { get; set; } = false;
    /// <summary>
    /// Name of rate limit policy. Default is "Quickie-Rl-Policy"
    /// </summary>
    public string? PolicyName { get; } = "Quickie-Rl-Policy";
    /// <summary>
    /// Number of request. Default is: 1 request every 'FromSeconds'
    /// </summary>
    public int? PermitLimit { get; set; } = 1;
    /// <summary>
    /// Duration (seconds) to block. Default is: 6
    /// </summary>
    public int? FromSeconds { get; set; } = 6;
}