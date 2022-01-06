using MediatR;

namespace Hflex.MediatR.Extensions.Caching.Services;

public interface ICacheKeyService
{
    (string baseKey, string key, ConfigurationItem config) GenerateDefaultKey<TRequest>(TRequest request) where TRequest: class, IBaseRequest;
    (string baseKey, ConfigurationItem config) GenerateBaseKeyOnly(Type requestType);
}