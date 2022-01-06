# Hflex.MediatR.Extensions.Caching 
Caching extension for MediatR

Inspired by https://github.com/Iamcerba/AspNetCore.CacheOutput


## Installation

1.  **Install-Package Hflex.MediatR.Extensions.Redis** for distributed cache via Redis or **Install-Package Hflex.MediatR.Extensions.InMemoryCaching** for in memory caching.
2. Define configurations for caching
  ```
  var cachingConfigurations = new CachingConfiguration();

  //GetTodoItemsQuery will be invalidated, when timer notification will be fired on InvalidateCacheHostedService
  cachingConfigurations.AddConfiguration<GetTodoItemsQuery>(duration: TimeSpan.FromMinutes(2), false);

  //GetWeatherForecastsQuery will be invalidated, when UpdateWeatherForecastsCommand called
  cachingConfigurations.AddConfiguration<GetWeatherForecastsQuery>(TimeSpan.FromMinutes(10), false, null, 
    typeof(UpdateWeatherForecastsCommand));
  ```
3. In Startup.cs or Program.cs Register accordingly
  - In case of Redis use
  `builder.Services.AddRedisCache(cachingConfigurations, builder.Configuration.GetConnectionString("RedisConnectionString"));`
  - In case of InMemory use
  `builder.Services.AddInMemoryCache(cachingConfigurations);`
  
  ## Usage
  
  Everything does configuration, just register
 ```
 var cachingConfigurations = new CachingConfiguration();
 cachingConfigurations.AddConfiguration<GetWeatherForecastsQuery>(duration: TimeSpan.FromMinutes(10),//Cache duration
 perUser: false, //Set to true for caching per user (includes userID into cache key)
 keyFactory: null, //Create Func<string> for replacing default key, which is serialized json from request object
 invalidatesOnRequests: typeof(UpdateWeatherForecastsCommand), typeof(secondcommand), typeof(thirdCommand) ..... //Command types to invalidate query);
 ``` 
 
 like this for GetWeatherForecastsQuery query, which will get cached for 10 minutes. And once any of invalidatesOnRequests called it will get invalidated automatically.
  Alternative way of force invalidating queries is to use Notifications. Just publish ` _mediator.Publish(new InvalidateCacheNotification { RequestType = typeof(queryType)});` anywhere in the application e.g. in samples I use hostedService with timer to simulate some domain events, which invalidates GetTodoItemsQuery 
  ` _mediator.Publish(new InvalidateCacheNotification { RequestType = typeof(GetTodoItemsQuery)});`
  
