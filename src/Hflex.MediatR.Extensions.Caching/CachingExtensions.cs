using System.Linq.Expressions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Hflex.MediatR.Extensions.Caching;

public static class CachingExtensions
{
    public static IServiceCollection AddMediatRCache(this IServiceCollection services,
        CachingConfiguration cachingConfiguration)
    {
        services.AddMemoryCache();
        foreach (var configurationItem in cachingConfiguration.ConfigurationItems)
        {
            var requestType = configurationItem.Type;
            var responseType = requestType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>)).GetGenericArguments()[0];
            var genericBaseInterface = typeof(IPipelineBehavior<,>);
            var pipeLineBehaviorInterface = genericBaseInterface.MakeGenericType(requestType, responseType);
                
            var genericCachingBehaviorBase = typeof(CachingBehavior<,>);
            var pipeLineBehaviorCaching = genericCachingBehaviorBase.MakeGenericType(requestType, responseType);
            services.AddTransient(pipeLineBehaviorInterface, pipeLineBehaviorCaching);
        }
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        return services;
    }
}