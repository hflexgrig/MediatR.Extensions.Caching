using Hflex.MediatR.Extensions.Caching.Interfaces;
using Hflex.MediatR.Extensions.Caching.Models;
using MediatR;

namespace Hflex.MediatR.Extensions.Caching.NotificationHandlers;

public class InvalidateCacheNotificationHandler:INotificationHandler<InvalidateCacheNotification> 
{
    private readonly IMediatorCaching _caching;

    public InvalidateCacheNotificationHandler(IMediatorCaching caching)
    {
        _caching = caching;
    }

    public Task Handle(InvalidateCacheNotification notification, CancellationToken cancellationToken)
    {
        var requestType = notification.RequestType;
        return _caching.InvalidateCacheAsync(requestType);
    }
}