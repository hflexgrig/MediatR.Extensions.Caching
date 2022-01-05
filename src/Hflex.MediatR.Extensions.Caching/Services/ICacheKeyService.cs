namespace Hflex.MediatR.Extensions.Caching.Services;

public interface ICacheKeyService
{
    (string baseKey, string key, ConfigurationItem config) GenerateDefaultKey<TRequest>(TRequest request);
}