using Hflex.MediatR.Extensions.Caching.Models;
using MediatR;
using WebApiSample.Application.TodoItems.Queries;
using WebApiSample.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace WebApiSample.HostedServices;

public class InvalidateCacheHostedService:BackgroundService
{
    private readonly IMediator _mediator;

    public InvalidateCacheHostedService(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new Timer(TimerCallback, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2));
        return Task.CompletedTask;
    }

    private void TimerCallback(object? state)
    {
        //Invalidate cache for GetTodoItemsQuery
        _mediator.Publish(new InvalidateCacheNotification { RequestTypes = new []{typeof(GetTodoItemsQuery)} });
    }
}