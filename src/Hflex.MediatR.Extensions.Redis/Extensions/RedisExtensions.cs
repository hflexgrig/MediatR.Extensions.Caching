using System.Text.Json;
using StackExchange.Redis;

namespace Hflex.MediatR.Extensions.Redis.Extensions;

internal static class StackExchangeRedisExtensions
{
    internal static async Task<T> GetAsync<T>(this IDatabase cache, string key)
    {
        RedisValue result = await cache.StringGetAsync(key);

        if (result.IsNull)
        {
            return default(T);
        }

        return JsonSerializer.Deserialize<T>(result);
    }

    internal static Task<bool> SetAsync(this IDatabase cache, string key, object value, TimeSpan? expiry = null)
    {
        return cache.StringSetAsync(key, JsonSerializer.Serialize(value), expiry);
    }
}