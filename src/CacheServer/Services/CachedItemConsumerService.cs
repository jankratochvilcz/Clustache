using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;

namespace Clustache.Services;

public class CachedItemConsumerService : BackgroundService
{
    private readonly ILogger<CachedItemConsumerService> _logger;
    private readonly RedisService _redisService;
    private readonly CachedItemService _cachedItemService;

    public CachedItemConsumerService(
        ILogger<CachedItemConsumerService> logger,
        RedisService redisService,
        CachedItemService cachedItemService
    )
    {
        _logger = logger;
        _redisService = redisService;
        _cachedItemService = cachedItemService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _redisService.SubscribeToChannel("cache-updates", HandleCacheUpdate);
        return Task.CompletedTask;
    }

    private void HandleCacheUpdate(string message)
    {
        var cachedItem = JsonSerializer.Deserialize<CachedItem>(message);
        if (cachedItem != null)
        {
            cachedItem.Origin = CachedItemOrigin.RedisSubscription;
            _cachedItemService.AddOrUpdateLocalCache(cachedItem, false);
            _logger.LogInformation($"Updated from subscription: {cachedItem.ItemId}");
        }
    }
}
