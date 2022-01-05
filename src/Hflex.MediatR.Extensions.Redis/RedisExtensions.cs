using Hflex.MediatR.Extensions.Caching;
using Hflex.MediatR.Extensions.Caching.Interfaces;
using Hflex.MediatR.Extensions.Caching.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace Hflex.MediatR.Extensions.Redis;

public static class RedisExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services,
        CachingConfiguration cachingConfiguration, string connectionString)
    {
        services.AddSingleton(cachingConfiguration);
        services.AddSingleton<ICacheKeyService, CacheKeyService>();
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.TryAdd(ServiceDescriptor.Singleton<IMediatorCaching, RedisCacheProvider>());
        
        services.TryAdd(ServiceDescriptor.Singleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString)));
        services.TryAdd(ServiceDescriptor.Transient<IDatabase>(e => e.GetRequiredService<IConnectionMultiplexer>().GetDatabase(-1, null)));

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