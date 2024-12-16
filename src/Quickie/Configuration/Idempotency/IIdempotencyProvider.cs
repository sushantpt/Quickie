namespace Quickie.Configuration.Idempotency;

/// <summary>
/// Idempotency provider contract
/// </summary>
public interface IIdempotencyProvider
{
    /// <summary>
    /// Check if the <c>key</c> is already used.
    /// </summary>
    /// <param name="key">Unique key</param>
    /// <returns>whether the key is used or not.</returns>
    ValueTask<bool> ExistsAsync(string key);
    
    /// <summary>
    /// Add and mark <c>key</c> as used.
    /// </summary>
    /// <param name="key">Unique key</param>
    /// <returns>a Task</returns>
    ValueTask MarkAsync(string key);
    
    /// <summary>
    /// Remove idempotency keys.
    /// </summary>
    /// <returns></returns>
    ValueTask RemoveExpiredKeys();
}