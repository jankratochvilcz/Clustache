using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Clustache.Services
{
    public class RedisConfiguration
    {
        public string ConnectionString { get; init; }
    }

    public class RedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly ISubscriber _subscriber;

        public RedisService(RedisConfiguration configuration)
        {
            _redis = ConnectionMultiplexer.Connect(configuration.ConnectionString);
            _db = _redis.GetDatabase();
            _subscriber = _redis.GetSubscriber();
        }

        public async Task PublishMessageAsync(string channel, string message)
        {
            await _subscriber.PublishAsync(channel, message);
        }

        public async Task SetItemAsync(string key, string value, TimeSpan expiration)
        {
            await _db.StringSetAsync(key, value, expiration);
        }

        public async Task<string> GetItemAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }

        public void SubscribeToChannel(string channel, Action<string> messageHandler)
        {
            _subscriber.Subscribe(channel, (ch, message) => messageHandler(message));
        }
    }
}

