using MediatR;

namespace Hflex.MediatR.Extensions.Caching.Models;

public record InvalidateCacheNotification: INotification
{
    public Type[] RequestTypes { get; init; } = {};
}