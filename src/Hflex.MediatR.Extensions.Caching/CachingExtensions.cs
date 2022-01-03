using System.Linq.Expressions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hflex.MediatR.Extensions.Caching;

public static class CachingExtensions
{
    public static IServiceCollection AddMediatRInMemoryCache(this IServiceCollection services,
        CachingConfiguration cachingConfiguration)
    {
        services.AddSingleton(cachingConfiguration);
        
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.TryAdd(ServiceDescriptor.Singleton<IMediatorCaching, InMemoryCachingProvider>());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(InMemoryCachingInvalidateBehavior<,>));

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
                
            var genericCachingBehaviorBase = typeof(InMemoryCachingBehavior<,>);
            var pipeLineBehaviorCaching = genericCachingBehaviorBase.MakeGenericType(requestType, responseType);
            services.TryAddTransient(pipeLineBehaviorInterface, pipeLineBehaviorCaching);
        }

        services.AddSingleton(invalidateCacheTypesDictionary);
        return services;
    }
    
    public static IServiceCollection AddMediatRDistributedCache(this IServiceCollection services,
        CachingConfiguration cachingConfiguration)
    {
        //services.AddHttpContextAccessor();
        //services.AddDistributedMemoryCache();
        //services.AddMemoryCache();
        services.AddSingleton<IMediatorCaching, InMemoryCachingProvider>();
        // foreach (var configurationItem in cachingConfiguration.ConfigurationItems)
        // {
        //     var requestType = configurationItem.Type;
        //     var responseType = requestType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>)).GetGenericArguments()[0];
        //     var genericBaseInterface = typeof(IPipelineBehavior<,>);
        //     var pipeLineBehaviorInterface = genericBaseInterface.MakeGenericType(requestType, responseType);
        //         
        //     var genericCachingBehaviorBase = typeof(InMemoryCachingBehavior<,>);
        //     var pipeLineBehaviorCaching = genericCachingBehaviorBase.MakeGenericType(requestType, responseType);
        //     services.TryAddTransient(pipeLineBehaviorInterface, pipeLineBehaviorCaching);
        // }
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        return services;
    }
}