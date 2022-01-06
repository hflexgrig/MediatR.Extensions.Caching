using System.Reflection;
using Hflex.MediatR.Extensions.Caching;
using Hflex.MediatR.Extensions.InMemoryCaching;
using Hflex.MediatR.Extensions.Redis;
using MediatR;
using WebApiSample.Application.Common.Repository;
using WebApiSample.Application.TodoItems.Commands;
using WebApiSample.Application.TodoItems.Queries;
using WebApiSample.Application.WeatherForecasts.Commands.UpdateWeatherForecasts;
using WebApiSample.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using WebApiSample.HostedServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<TodoItemsRepository>();
var cachingConfigurations = new CachingConfiguration();

//GetTodoItemsQuery will be invalidated, when timer notification will be fired on InvalidateCacheHostedService
cachingConfigurations.AddConfiguration<GetTodoItemsQuery>(duration: TimeSpan.FromMinutes(2), false, query => $"{query.Size}_{query.Page}");

//GetWeatherForecastsQuery will be invalidated, when UpdateWeatherForecastsCommand called
cachingConfigurations.AddConfiguration<GetWeatherForecastsQuery>(TimeSpan.FromMinutes(10), false, null, 
    typeof(UpdateWeatherForecastsCommand));

//Add Redis cache with caching configuration.
builder.Services.AddRedisCache(cachingConfigurations, builder.Configuration.GetConnectionString("RedisConnectionString"));

//Register hosted service, where timer works sending notification for invalidating GetTodoItemsQuery cache
builder.Services.AddHostedService<InvalidateCacheHostedService>();

//builder.Services.AddInMemoryCache(cachingConfigurations);

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
