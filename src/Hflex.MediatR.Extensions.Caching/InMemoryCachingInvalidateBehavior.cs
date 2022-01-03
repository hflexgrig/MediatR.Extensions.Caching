using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Hflex.MediatR.Extensions.Caching;

public class InMemoryCachingInvalidateBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where
    TRequest : IRequest<TResponse>
{
    private readonly IMediatorCaching _caching;
    private readonly InvalidateCacheRequests _invalidateCacheRequests;

    public InMemoryCachingInvalidateBehavior(IMediatorCaching caching, InvalidateCacheRequests invalidateCacheRequests)
    {
        _caching = caching;
        _invalidateCacheRequests = invalidateCacheRequests;
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        //If this is command, which invalidates it's queries, then we invalidate cache for all queries
        if (_invalidateCacheRequests.TryGetValue(typeof(TRequest), out var queryRequests))
        {
            foreach (var queryRequest in queryRequests)
            {
                _caching.InvalidateCacheAsync(queryRequest);
            }
        }
        return next();
    }
}