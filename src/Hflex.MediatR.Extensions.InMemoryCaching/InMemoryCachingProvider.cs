using System.Text.Json;
using Hflex.MediatR.Extensions.Caching;
using Hflex.MediatR.Extensions.Caching.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Hflex.MediatR.Extensions.InMemoryCaching;

public class InMemoryCachingProvider : IMediatorCaching
{
    private readonly IMemoryCache _memoryCache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CachingConfiguration _cachingConfiguration;
    private const string CancellationTokenKey = ":cts";

    public InMemoryCachingProvider(IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor,
        CachingConfiguration cachingConfiguration)
    {
        _memoryCache = memoryCache;
        _httpContextAccessor = httpContextAccessor;
        _cachingConfiguration = cachingConfiguration;
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
        _cachingConfiguration.TryGetValue(queryRequestType, out var config);
        var identityString =
            config.PerUser ? $"user:{_httpContextAccessor.HttpContext?.User?.Identity?.Name};" : string.Empty;

        var baseKey = $"class:{queryRequestType.FullName};{identityString}";

        return RemoveAsync(baseKey);
    }

    public async Task<TResponse> GetOrAddAsync<TRequest, TResponse>(TRequest request,
        Func<Task<TResponse>> valueFactory) where TResponse : class
    {
        _cachingConfiguration.TryGetValue(typeof(TRequest), out var config);
        var identityString =
            config.PerUser ? $"user:{_httpContextAccessor.HttpContext?.User?.Identity?.Name};" : string.Empty;

        var baseKey = $"class:{typeof(TRequest).FullName};{identityString}";
        var key = $"token:{JsonSerializer.Serialize(request)}";
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

