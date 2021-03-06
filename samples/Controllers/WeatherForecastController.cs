using Microsoft.AspNetCore.Mvc;
using WebApiSample.Application.TodoItems.Commands;
using WebApiSample.Application.TodoItems.Queries;
using WebApiSample.Application.WeatherForecasts.Commands.UpdateWeatherForecasts;
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
    public Task<List<WeatherForecast>> Get([FromQuery] GetWeatherForecastsQuery getWeatherForecastsQuery)
    {
        return _mediator.Send(getWeatherForecastsQuery);
    }

    [HttpPut]
    public Task<Unit> Put()
    {
        return _mediator.Send(new UpdateWeatherForecastsCommand());
    }
}
