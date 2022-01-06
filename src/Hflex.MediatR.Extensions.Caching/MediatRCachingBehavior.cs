using Hflex.MediatR.Extensions.Caching.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Hflex.MediatR.Extensions.Caching;

public class MediatRCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where
    TRequest : class, IRequest<TResponse>
    where TResponse : class
{
    private readonly IMediatorCaching _caching;

    public MediatRCachingBehavior(IMediatorCaching caching)
    {
        _caching = caching;
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        
        return _caching.GetOrAddAsync(request, () => next());
    }
}