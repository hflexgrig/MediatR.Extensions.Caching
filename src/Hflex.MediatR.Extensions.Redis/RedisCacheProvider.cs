using System.Net;
using Hflex.MediatR.Extensions.Caching.Interfaces;
using Hflex.MediatR.Extensions.Caching.Services;
using Hflex.MediatR.Extensions.Redis.Extensions;
using StackExchange.Redis;

namespace Hflex.MediatR.Extensions.Redis;

public class RedisCacheProvider : IMediatorCaching
{
    private readonly IDatabase _redisCache;
    private readonly ICacheKeyService _cacheKeyService;

    public RedisCacheProvider(IDatabase redisCache,
        ICacheKeyService cacheKeyService)
    {
        _redisCache = redisCache;
        _cacheKeyService = cacheKeyService;
    }

    public async Task RemoveStartsWithAsync(string key)
    {
        if (key.Contains("*"))
        {
            // Partial cache invalidation using wildcards
            EndPoint[] endPoints = _redisCache.Multiplexer.GetEndPoints();

            foreach (EndPoint endPoint in endPoints)
            {
                IServer server = _redisCache.Multiplexer.GetServer(endPoint);

                IList<RedisKey> keys = server
                    .Keys(pattern: $"{key}")
                    .ToList();

                foreach (RedisKey memberKey in keys)
                {
                    await RemoveAsync(memberKey);
                }
            }
        }
        else
        {
            RedisValue[] keys = await _redisCache.SetMembersAsync(key);

            foreach (RedisValue memberKey in keys)
            {
                await RemoveAsync(memberKey);
            }

            await RemoveAsync(key);
        }
    }

    public async Task<T> GetAsync<T>(string key) where T : class
    {
        T result = await _redisCache.GetAsync<T>(key);

        if (typeof(T) == typeof(byte[]))
        {
            // GZip decompression
            return (T) Convert.ChangeType(((byte[]) (object) result).Decompress(), typeof(T));
        }

        return result;
    }

    public Task RemoveAsync(string key)
    {
        return _redisCache.KeyDeleteAsync(key);
    }

    public async Task<bool> ContainsAsync(string key)
    {
        if (key.Contains("*"))
        {
            EndPoint[] endPoints = _redisCache.Multiplexer.GetEndPoints();

            foreach (EndPoint endPoint in endPoints)
            {
                IServer server = _redisCache.Multiplexer.GetServer(endPoint);

                IList<RedisKey> keys = server
                    .Keys(pattern: $"{key}")
                    .ToList();

                if (keys.Any())
                {
                    return true;
                }
            }

            return false;
        }

        return await _redisCache.KeyExistsAsync(key);
    }

    public async Task AddAsync(string baseKey, string key, object value, DateTimeOffset expiration)
    {
        // Lets not store the base type (will be dependsOnKey later) since we want to use it as a set!
        if (Equals(value, string.Empty))
        {
            return;
        }

        byte[] byteArray = value as byte[];

        if (byteArray != null)
        {
            // GZip compression 
            value = byteArray.Compress();
        }

        bool primaryAdded = await _redisCache.SetAsync($"{baseKey}{key}", value, expiration.Subtract(DateTimeOffset.Now));
    }

    public async Task<TResponse> GetOrAddAsync<TRequest, TResponse>(TRequest request, Func<Task<TResponse>> valueFactory)
        where TResponse : class
    {
        var (baseKey, key, config) = _cacheKeyService.GenerateDefaultKey(request);
        
        var cached = await _redisCache.GetAsync<TResponse?>($"{baseKey}{key}");
        if (cached == null)
        {
            cached = await valueFactory();
            var expiration = DateTimeOffset.Now.Add(config.Duration);

            await AddAsync(baseKey, key, cached, expiration);
        }

        return cached;
    }

    public Task InvalidateCacheAsync(Type queryRequestType)
    {
        var (baseKey, _) = _cacheKeyService.GenerateBaseKeyOnly(queryRequestType);

        return RemoveStartsWithAsync($"{baseKey}*");
    }
}