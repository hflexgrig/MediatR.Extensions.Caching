using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Hflex.MediatR.Extensions.Caching;

public class InMemoryCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where
    TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IMediatorCaching _caching;

    public InMemoryCachingBehavior(IMediatorCaching caching)
    {
        _caching = caching;
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        
        return _caching.GetOrAddAsync(request, () => next());
    }
}