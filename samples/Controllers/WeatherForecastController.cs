using Microsoft.AspNetCore.Mvc;
using WebApiSample.Application.TodoItem.Commands;
using WebApiSample.Application.TodoItem.Queries;
using WebApiSample.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using WebApiSample.Models;
namespace MediatR.Extensions.Caching.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMediator _mediator;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public Task<TodoItemDto> Get()
    {
        return _mediator.Send(new GetTodoItemQuery());
    }
    
    [HttpGet("forecasts")]
    public Task<List<WeatherForecast>> GetWeatherForecasts([FromQuery] GetWeatherForecastsQuery getWeatherForecastsQuery)
    {
        return _mediator.Send(getWeatherForecastsQuery);
    }

    [HttpPost]
    public Task<Unit> Post(CreateTodoItemCommand createTodoItemCommand)
    {
        return _mediator.Send(createTodoItemCommand);
    }
}
