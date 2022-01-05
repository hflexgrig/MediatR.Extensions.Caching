using MediatR;
namespace Hflex.MediatR.Extensions.Caching;

public class CachingConfiguration:Dictionary<Type, ConfigurationItem>
{
    public void AddConfiguration<TRequest>(TimeSpan duration, bool perUser = false, Func<string>? keyFactory = null, params Type[] InvalidatesOnRequests) where TRequest: IBaseRequest
    {
        Add(typeof(TRequest), new ConfigurationItem{ Duration = duration, PerUser = perUser, KeyFactory = keyFactory, InvalidatesOnRequests = InvalidatesOnRequests});
    }
}

public record ConfigurationItem
{
    public TimeSpan Duration { get; init; }
    public bool PerUser { get; init; }

    public IList<Type> InvalidatesOnRequests { get; init; }
    
    public Func<string>? KeyFactory { get; init; }
}
