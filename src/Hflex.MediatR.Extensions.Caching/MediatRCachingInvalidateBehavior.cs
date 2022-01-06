using Hflex.MediatR.Extensions.Caching.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Hflex.MediatR.Extensions.Caching;

public class MediatRCachingInvalidateBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where
    TRequest : IRequest<TResponse>
{
    private readonly IMediatorCaching _caching;
    private readonly InvalidateCacheRequests _invalidateCacheRequests;

    public MediatRCachingInvalidateBehavior(IMediatorCaching caching, InvalidateCacheRequests invalidateCacheRequests)
    {
        _caching = caching;
        _invalidateCacheRequests = invalidateCacheRequests;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
       
        bool isException = false;

        try
        {
            return await next();
        }
        catch (Exception e)
        {
            isException = true;
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            if (!isException)
            {
                //If this is command, which invalidates it's queries, then we invalidate cache for all queries
                if (_invalidateCacheRequests.TryGetValue(typeof(TRequest), out var queryRequests))
                {
                    foreach (var queryRequest in queryRequests)
                    {
                        _caching.InvalidateCacheAsync(queryRequest);
                    }
                }
            }
        }
        
    }
}