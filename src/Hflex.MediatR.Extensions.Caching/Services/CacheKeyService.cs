using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Hflex.MediatR.Extensions.Caching.Services;

public class CacheKeyService : ICacheKeyService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CachingConfiguration _cachingConfiguration;

    public CacheKeyService(IHttpContextAccessor httpContextAccessor,
        CachingConfiguration cachingConfiguration)
    {
        _httpContextAccessor = httpContextAccessor;
        _cachingConfiguration = cachingConfiguration;
    }

    public (string baseKey, string key, ConfigurationItem config) GenerateDefaultKey<TRequest>(TRequest request)
    {
        _cachingConfiguration.TryGetValue(typeof(TRequest), out var config);
        var identityString =
            config.PerUser ? $"user:{_httpContextAccessor.HttpContext?.User?.Identity?.Name};" : string.Empty;

        var baseKey = $"class:{typeof(TRequest).FullName};{identityString}";
        var key = config.KeyFactory == null ? $"token:{JsonSerializer.Serialize(request)}" : config.KeyFactory();

        return (baseKey, key, config);
    }
    
    public (string baseKey, ConfigurationItem config) GenerateBaseKeyOnly(Type requestType)
    {
        _cachingConfiguration.TryGetValue(requestType, out var config);
        var identityString =
            config.PerUser ? $"user:{_httpContextAccessor.HttpContext?.User?.Identity?.Name};" : string.Empty;

        var baseKey = $"class:{requestType.FullName};{identityString}";

        return (baseKey, config);
    }
}