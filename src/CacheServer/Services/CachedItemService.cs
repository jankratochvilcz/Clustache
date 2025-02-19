using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models;
using Prometheus;

namespace Clustache.Services;

public class CachedItemService(ILogger<CachedItemService> logger, RedisService redisService, DatabaseService databaseService)
{
    private const int ArtificialCacheFillFromSourceDelay = 1000;
    private static readonly Counter FilledFromSourceMetric = Metrics.CreateCounter(
        "clustache_cache_filled_from_source",
        "Number of cache items filled from source"
    );
    private static readonly Counter FilledFromCacheOwnMetric = Metrics.CreateCounter(
        "clustache_cache_filled_from_cache_own",
        "Number of cache items filled from cache"
    );
    private static readonly Counter FilledFromCacheSubscriptionMetric = Metrics.CreateCounter(
        "clustache_cache_filled_from_cache_subscription",
        "Number of cache items filled from cache"
    );

    private readonly ConcurrentDictionary<string, CachedItem> _cache = new();
    private readonly ILogger<CachedItemService> logger = logger;
    private readonly RedisService redisService = redisService;
    private readonly DatabaseService databaseService = databaseService;

    public async Task<CachedItem> GetItem(string itemId)
    {
        // Gets from cache
        if (_cache.TryGetValue(itemId, out var cachedItem))
        {
            logger.LogInformation($"Retrieved item from cache: {itemId}");

            switch (cachedItem)
            {
                case { Origin: CachedItemOrigin.RedisSubscription }:
                    FilledFromCacheSubscriptionMetric.Inc(1);
                    break;
                case { Origin: CachedItemOrigin.Source }:
                    FilledFromCacheOwnMetric.Inc(1);
                    break;
                default:
                    break;
            }

            return cachedItem;
        }

        var itemFromSource = await databaseService.GetItem(itemId);
        itemFromSource.Origin = CachedItemOrigin.Source;

        logger.LogInformation($"Retrieved item from source with ID {itemId}: {JsonSerializer.Serialize(itemFromSource)}");
        AddOrUpdateLocalCache(itemFromSource);
        FilledFromSourceMetric.Inc();

        return itemFromSource;
    }

    /// <summary>
    /// To side-load the cache with items from Redis
    /// </summary>
    public void AddOrUpdateLocalCache(CachedItem item, bool broadcastCachedItem = true)
    {
        // I don't want to marked items that got back to me from Redis that I added to keep better track of things.
        if (_cache.ContainsKey(item.ItemId))
            return;

        _cache[item.ItemId] = item;

        if (broadcastCachedItem)
            BroadcastCachedItem(item);
    }

    private void BroadcastCachedItem(CachedItem item)
    {
        var message = JsonSerializer.Serialize(item);
        Task.Run(async () =>
        {
            await redisService.PublishMessageAsync("cache-updates", message);
        });
    }
}
