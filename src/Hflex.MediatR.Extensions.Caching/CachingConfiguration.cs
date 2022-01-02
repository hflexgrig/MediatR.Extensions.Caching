using MediatR;
namespace Hflex.MediatR.Extensions.Caching;

public class CachingConfiguration
{
    
    public CachingConfiguration()
    {
        
    }

    public void AddConfiguration<TRequest>(TimeSpan timeout) where TRequest: IBaseRequest
    {
        ConfigurationItems.Add(new ConfigurationItem{Type = typeof(TRequest), Timeout = timeout});
    }

    public ICollection<ConfigurationItem> ConfigurationItems { get; } = new List<ConfigurationItem>();
}

public record ConfigurationItem
{
    public Type Type { get; init; }
    public TimeSpan Timeout { get; init; }
}