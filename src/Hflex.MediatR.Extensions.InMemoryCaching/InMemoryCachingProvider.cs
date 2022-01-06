using Hflex.MediatR.Extensions.Caching.Interfaces;
using Hflex.MediatR.Extensions.Caching.Services;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Hflex.MediatR.Extensions.InMemoryCaching;

public class InMemoryCachingProvider : IMediatorCaching
{
    private readonly IMemoryCache _memoryCache;
    private readonly ICacheKeyService _cacheKeyService;
    private const string CancellationTokenKey = ":cts";

    public InMemoryCachingProvider(IMemoryCache memoryCache,
        ICacheKeyService cacheKeyService)
    {
        _memoryCache = memoryCache;
        _cacheKeyService = cacheKeyService;
    }

    public Task RemoveStartsWithAsync(string key)
    {
        return RemoveAsync(key);
    }

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        return Task.FromResult(_memoryCache.Get(key) as T);
    }

    public Task RemoveAsync(string key)
    {
        if (_memoryCache.TryGetValue($"{key}{CancellationTokenKey}", out CancellationTokenSource cts))
        {
            cts.Cancel();
        }
        else
        {
            _memoryCache.Remove(key);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ContainsAsync(string key)
    {
        return Task.FromResult(_memoryCache.TryGetValue(key, out _));
    }

    public Task AddAsync(string baseKey, string key, object value, DateTimeOffset expiration)
    {
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(expiration.Subtract(DateTimeOffset.Now));

        if (_memoryCache.TryGetValue($"{baseKey}{CancellationTokenKey}", out CancellationTokenSource existingCts))
        {
            options.AddExpirationToken(new CancellationChangeToken(existingCts.Token));
        }
        else
        {
            var cts = new CancellationTokenSource();

            options.AddExpirationToken(new CancellationChangeToken(cts.Token));

            _memoryCache.Set($"{baseKey}{CancellationTokenKey}", cts, options);
        }

        _memoryCache.Set($"{baseKey}{key}", value, options);

        return Task.CompletedTask;
    }

    public Task InvalidateCacheAsync(Type queryRequestType)
    {
        var (baseKey, _) = _cacheKeyService.GenerateBaseKeyOnly(queryRequestType);

        return RemoveAsync(baseKey);
    }

    public async Task<TResponse> GetOrAddAsync<TRequest, TResponse>(TRequest request,
        Func<Task<TResponse>> valueFactory)
        where  TRequest: class, IBaseRequest
        where TResponse : class
    {
        var (baseKey, key, config) = _cacheKeyService.GenerateDefaultKey(request);

        var cached = _memoryCache.Get<TResponse?>($"{baseKey}{key}");
        if (cached == null)
        {
            cached = await valueFactory();
            var expiration = DateTimeOffset.Now.Add(config.Duration);

            await AddAsync(baseKey, key, cached, expiration);
        }

        return cached;
    }
}

