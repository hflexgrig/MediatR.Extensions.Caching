namespace Hflex.MediatR.Extensions.Caching.Services;

public interface ICacheKeyService
{
    (string baseKey, string key, ConfigurationItem config) GenerateDefaultKey<TRequest>(TRequest request);
    (string baseKey, ConfigurationItem config) GenerateBaseKeyOnly(Type requestType);
}