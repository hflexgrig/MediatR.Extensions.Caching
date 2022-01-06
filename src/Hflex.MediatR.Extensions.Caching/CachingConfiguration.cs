using MediatR;
namespace Hflex.MediatR.Extensions.Caching;

public class CachingConfiguration:Dictionary<Type, ConfigurationItem>
{
    public void AddConfiguration<TRequest>(TimeSpan duration, bool perUser = false, Func<TRequest, string>? keyFactory = null, params Type[] InvalidatesOnRequests) where TRequest: class, IBaseRequest
    {
        Add(typeof(TRequest), new ConfigurationItem{ Duration = duration, PerUser = perUser, KeyFactory = keyFactory == null ? null : (obj) => keyFactory.Invoke(obj as TRequest), InvalidatesOnRequests = InvalidatesOnRequests});
    }
}

public record ConfigurationItem
{
    public TimeSpan Duration { get; init; }
    public bool PerUser { get; init; }

    public IList<Type> InvalidatesOnRequests { get; init; }
    
    public Func<IBaseRequest, string>? KeyFactory { get; init; }
}

