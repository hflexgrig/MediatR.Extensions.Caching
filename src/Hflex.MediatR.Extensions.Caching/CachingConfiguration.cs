using MediatR;
namespace Hflex.MediatR.Extensions.Caching;

public class CachingConfiguration:Dictionary<Type, ConfigurationItem>
{
    
    public CachingConfiguration()
    {
        
    }


    public void AddConfiguration<TRequest>(TimeSpan duration, bool perUser, params Type[] InvalidatesOnRequests) where TRequest: IBaseRequest
    {
        Add(typeof(TRequest), new ConfigurationItem{ Duration = duration, PerUser = perUser, InvalidatesOnRequests = InvalidatesOnRequests});
    }
}

public record ConfigurationItem
{
    public TimeSpan Duration { get; init; }
    public bool PerUser { get; init; }

    public IList<Type> InvalidatesOnRequests { get; init; }
};
