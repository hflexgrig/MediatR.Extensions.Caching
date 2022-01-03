using System.Reflection;
using Hflex.MediatR.Extensions.Caching;
using MediatR;
using WebApiSample.Application.TodoItem.Queries;
using WebApiSample.Application.WeatherForecasts.Queries.GetWeatherForecasts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddHttpContextAccessor();

var cachingConfigurations = new CachingConfiguration();
cachingConfigurations.AddConfiguration<GetTodoItemQuery>(TimeSpan.FromMinutes(2));
cachingConfigurations.AddConfiguration<GetWeatherForecastsQuery>(TimeSpan.FromMinutes(2));

builder.Services.AddMediatRInMemoryCache(cachingConfigurations);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
