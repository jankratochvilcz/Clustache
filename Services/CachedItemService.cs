using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;

namespace Clustache.Services
{
    public class CachedItemService
    {
        private const int ArtificialCacheFillFromSourceDelay = 1000;
        private static Counter FilledFromSourceMetric = Metrics.CreateCounter("clustache_cache_filled_from_source", "Number of cache items filled from source");
        private static Counter FilledFromCacheOwnMetric = Metrics.CreateCounter("clustache_cache_filled_from_cache_own", "Number of cache items filled from cache");
        private static Counter FilledFromCacheSubscriptionMetric = Metrics.CreateCounter("clustache_cache_filled_from_cache_subscription", "Number of cache items filled from cache");

        private readonly ConcurrentDictionary<string, CachedItem> _cache = new();
        private readonly ILogger<CachedItemService> _logger;
        private readonly RedisService _redisService;

        public CachedItemService(ILogger<CachedItemService> logger, RedisService _redisService)
        {
            _logger = logger;
            this._redisService = _redisService;
        }

        public async Task<CachedItem> GetItem(string itemId)
        {

            // Gets from cache
            if (_cache.TryGetValue(itemId, out var cachedItem))
            {
                _logger.LogInformation($"Retrieved item from cache: {itemId}");

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

            // Needs to fill the cache first, simulate a delay
            await Task.Delay(ArtificialCacheFillFromSourceDelay);

            CachedItem itemFromSource = new CachedItem { ItemId = itemId, ItemValue = "demovalue", Origin = CachedItemOrigin.Source };
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

            if(broadcastCachedItem)
                BroadcastCachedItem(item);
        }

        private void BroadcastCachedItem(CachedItem item)
        {
            var message = JsonSerializer.Serialize(item);
            Task.Run(async () =>
            {
                await _redisService.PublishMessageAsync("cache-updates", message);
            });
        }
    }
}