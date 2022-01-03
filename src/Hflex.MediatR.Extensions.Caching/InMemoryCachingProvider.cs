using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Hflex.MediatR.Extensions.Caching;

public class InMemoryCachingProvider : IMediatorCaching
{
    private readonly IMemoryCache _memoryCache;
    private const string CancellationTokenKey = ":cts";

    public InMemoryCachingProvider(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
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

    public Task AddAsync(string key, object value, DateTimeOffset expiration, string? dependsOnKey = null)
    {
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(expiration.Subtract(DateTimeOffset.Now));

        if (string.IsNullOrEmpty(dependsOnKey))
        {
            if (_memoryCache.TryGetValue($"{key}{CancellationTokenKey}", out CancellationTokenSource existingCts))
            {
                options.AddExpirationToken(new CancellationChangeToken(existingCts.Token));
            }
            else
            {
                var cts = new CancellationTokenSource();

                options.AddExpirationToken(new CancellationChangeToken(cts.Token));

                _memoryCache.Set($"{key}{CancellationTokenKey}", cts, options);
            }

            _memoryCache.Set(key, value, options);
        }
        else
        {
            if (_memoryCache.TryGetValue($"{dependsOnKey}{CancellationTokenKey}",
                    out CancellationTokenSource existingCts))
            {
                options.AddExpirationToken(new CancellationChangeToken(existingCts.Token));

                _memoryCache.Set(key, value, options);
            }
        }

        return Task.CompletedTask;
    }

    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> valueFactory,DateTimeOffset expiration, string? dependsOnKey = null) where T:class
    {
        var cached = _memoryCache.Get<T?>(key);
        if (cached == null)
        {
            cached = await valueFactory();
            await AddAsync(key, cached, expiration, dependsOnKey);
        }

        return cached;
    }
}

public interface IMediatorCaching
{
    Task RemoveStartsWithAsync(string key);
    Task<T?> GetAsync<T>(string key) where T : class;
    Task RemoveAsync(string key);
    Task<bool> ContainsAsync(string key);
    Task AddAsync(string key, object value, DateTimeOffset expiration, string? dependsOnKey = null);
    Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> valueFactory,DateTimeOffset expiration, string? dependsOnKey = null) where T:class;
}