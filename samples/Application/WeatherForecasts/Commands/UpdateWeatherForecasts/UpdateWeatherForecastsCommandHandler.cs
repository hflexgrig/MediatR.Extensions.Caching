using MediatR;

namespace WebApiSample.Application.WeatherForecasts.Commands.UpdateWeatherForecasts;

public class UpdateWeatherForecastsCommandHandler:IRequestHandler<UpdateWeatherForecastsCommand, Unit>
{
    public Task<Unit> Handle(UpdateWeatherForecastsCommand request, CancellationToken cancellationToken)
    {
        var result = new Unit();
        return Task.FromResult(result);
    }
}