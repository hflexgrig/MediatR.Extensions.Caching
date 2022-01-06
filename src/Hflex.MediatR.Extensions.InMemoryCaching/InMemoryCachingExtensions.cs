using Hflex.MediatR.Extensions.Caching;
using Hflex.MediatR.Extensions.Caching.Interfaces;
using Hflex.MediatR.Extensions.Caching.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hflex.MediatR.Extensions.InMemoryCaching;

public static class InMemoryCachingExtensions
{
    public static IServiceCollection AddInMemoryCache(this IServiceCollection services,
        CachingConfiguration cachingConfiguration)
    {
        services.AddMediatR(typeof(CachingConfiguration).Assembly);
        services.AddSingleton(cachingConfiguration);
        services.AddSingleton<ICacheKeyService, CacheKeyService>();

        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.TryAdd(ServiceDescriptor.Singleton<IMediatorCaching, InMemoryCachingProvider>());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MediatRCachingInvalidateBehavior<,>));

        var invalidateCacheTypesDictionary = new InvalidateCacheRequests();
        foreach (var configurationItem in cachingConfiguration)
        {
            foreach (var valueInvalidatesOnRequest in configurationItem.Value.InvalidatesOnRequests)
            {
                if (!invalidateCacheTypesDictionary.TryGetValue(valueInvalidatesOnRequest, out var invalidateQueies))
                {
                    invalidateQueies = new HashSet<Type>();
                    invalidateCacheTypesDictionary[valueInvalidatesOnRequest] = invalidateQueies;
                }

                invalidateQueies.Add(configurationItem.Key);
            }
           
            var requestType = configurationItem.Key;
            var responseType = requestType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>)).GetGenericArguments()[0];
            var genericBaseInterface = typeof(IPipelineBehavior<,>);
            var pipeLineBehaviorInterface = genericBaseInterface.MakeGenericType(requestType, responseType);
                
            var genericCachingBehaviorBase = typeof(MediatRCachingBehavior<,>);
            var pipeLineBehaviorCaching = genericCachingBehaviorBase.MakeGenericType(requestType, responseType);
            services.TryAddTransient(pipeLineBehaviorInterface, pipeLineBehaviorCaching);
        }

        services.AddSingleton(invalidateCacheTypesDictionary);
        return services;
    }
}