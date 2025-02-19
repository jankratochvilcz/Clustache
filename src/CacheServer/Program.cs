using System;
using Clustache.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var redisPort = int.Parse(Environment.GetEnvironmentVariable("REDIS_PORT"));
var redisPath = Environment.GetEnvironmentVariable("REDIS_PATH");
string redisConnectionString = $"{redisPath}:{redisPort}";
Console.WriteLine(redisConnectionString);

builder
    .Services.AddSingleton<CachedItemService>()
    .AddSingleton(new RedisConfiguration { ConnectionString = redisConnectionString })
    .AddSingleton<RedisService>();

builder.Services.AddHostedService<CachedItemConsumerService>();

var app = builder.Build();

app.MapControllers();
app.UseHttpMetrics();
app.MapMetrics();

app.Run();
