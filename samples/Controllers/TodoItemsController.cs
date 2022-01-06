using Microsoft.AspNetCore.Mvc;
using WebApiSample.Application.TodoItems.Commands;
using WebApiSample.Application.TodoItems.Queries;
using WebApiSample.Application.WeatherForecasts.Commands.UpdateWeatherForecasts;
using WebApiSample.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using WebApiSample.Models;
namespace MediatR.Extensions.Caching.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoItemsController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMediator _mediator;

    public TodoItemsController(ILogger<WeatherForecastController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public Task<List<TodoItemDto>> Get([FromQuery] GetTodoItemsQuery getTodoItemsQuery)
    {
        return _mediator.Send(getTodoItemsQuery);
    }
    
    [HttpPost]
    public Task<Unit> Post(CreateTodoItemCommand createTodoItemCommand)
    {
        return _mediator.Send(createTodoItemCommand);
    }
}
