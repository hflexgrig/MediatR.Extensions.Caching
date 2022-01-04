namespace Hflex.MediatR.Extensions.Caching.Interfaces;

public interface IMediatorCaching
{
    Task RemoveStartsWithAsync(string key);
    Task<T?> GetAsync<T>(string key) where T : class;
    Task RemoveAsync(string key);
    Task<bool> ContainsAsync(string key);
    Task AddAsync(string baseKey, string key, object value, DateTimeOffset expiration);

    Task<TResponse> GetOrAddAsync<TRequest, TResponse>(TRequest request, Func<Task<TResponse>> valueFactory)
        where TResponse : class;

    Task InvalidateCacheAsync(Type queryRequestType);
}