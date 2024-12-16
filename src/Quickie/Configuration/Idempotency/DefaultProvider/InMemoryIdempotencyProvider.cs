using System.Collections.Concurrent;

namespace Quickie.Configuration.Idempotency.DefaultProvider;

/// <summary>
/// Default InMemory idempotency provider
/// </summary>
public class InMemoryIdempotencyProvider : IIdempotencyProvider
{
    private readonly ConcurrentDictionary<string, DateTime> _keys = new();
    private readonly QuickieOptions _options = GlobalQuickieConfigData.Options; 

    /// <summary>
    /// Asynchronous method synchronous operation(check in dictionary).
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>bool</returns>
    public ValueTask<bool> ExistsAsync(string key)
    {
        return ValueTask.FromResult(_keys.ContainsKey(key));
    }

    /// <summary>
    /// Asynchronous method synchronous operation (add in dictionary).
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public ValueTask MarkAsync(string key)
    {
        _keys[key] = DateTime.UtcNow;
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Remove expired api idempotency keys
    /// </summary>
    /// <returns></returns>
    public ValueTask RemoveExpiredKeys()
    {
        var cutoffTime = DateTime.UtcNow.Subtract(_options.IdempotencyConfiguration?.IdempotencyLifespan ?? TimeSpan.FromHours(1));
        foreach (var key in _keys.Where(x => x.Value < cutoffTime).Select(x => x.Key))
        {
            _keys.TryRemove(key, out _);
        }
        
        return ValueTask.CompletedTask;
    }
}