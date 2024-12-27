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
        var lifespan = TimeSpan.FromHours(1);
        await _redisDatabase.StringSetAsync(key, DateTime.UtcNow.ToString("o"), lifespan);
    }

    public async ValueTask RemoveExpiredKeys()
    {
        // redis automatically removes keys when their lifespan.
        await ValueTask.CompletedTask;
    }
}