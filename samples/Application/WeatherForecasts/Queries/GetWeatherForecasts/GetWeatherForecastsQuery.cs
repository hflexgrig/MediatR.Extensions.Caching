using MediatR;
using WebApiSample.Models;

namespace WebApiSample.Application.WeatherForecasts.Queries.GetWeatherForecasts;

public class GetWeatherForecastsQuery:IRequest<List<WeatherForecast>>
{
    public int Page { get; set; }
}