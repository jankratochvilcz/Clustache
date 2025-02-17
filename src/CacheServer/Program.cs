using Clustache.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services
    .AddSingleton<CachedItemService>()
    .AddSingleton(new RedisConfiguration { ConnectionString = "localhost:51160" })
    .AddSingleton<RedisService>();

builder.Services.AddHostedService<CachedItemConsumerService>();


var app = builder.Build();

app.MapControllers();
app.UseHttpMetrics();
app.MapMetrics();

app.Run();
