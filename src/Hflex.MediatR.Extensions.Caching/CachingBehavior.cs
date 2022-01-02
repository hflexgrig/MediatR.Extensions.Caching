using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Hflex.MediatR.Extensions.Caching;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where
    TRequest : IRequest<TResponse>
{
    private readonly IMemoryCache _memoryCache;

    public CachingBehavior(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        return _memoryCache.GetOrCreateAsync(request.GetType().FullName, (b) => next());
    }
}