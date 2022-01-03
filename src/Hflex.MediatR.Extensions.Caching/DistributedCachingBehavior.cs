// using MediatR;
// using Microsoft.Extensions.Caching.Distributed;
// using Microsoft.Extensions.Caching.Memory;
//
// namespace Hflex.MediatR.Extensions.Caching;
//
// public class DistributedCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
//     where
//     TRequest : IRequest<TResponse>
// {
//     private readonly IDistributedCache _distributedCache;
//
//     public DistributedCachingBehavior(IDistributedCache memoryCache)
//     {
//         _distributedCache = memoryCache;
//     }
//
//     public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
//         RequestHandlerDelegate<TResponse> next)
//     {
//         var cachedItem = await _distributedCache.GetAsync(request.GetType().FullName, );
//         if ( == null)
//         {
//             _distributedCache.SetAsync(request.GetType().FullName, () => next())
//         }
//         return _distributedCache.set(request.GetType().FullName, (b) => next());
//     }
// }