using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AzureFromTheTrenches.Commanding.Cache.Redis.Implementation
{
    class RedisCacheAdapter : ICacheAdapter
    {
        private readonly Lazy<ConnectionMultiplexer> _multiplexer;

        public RedisCacheAdapter(Lazy<ConnectionMultiplexer> multiplexer)
        {
            _multiplexer = multiplexer;
        }

        public async Task Set(string key, object value, TimeSpan lifeTime)
        {
            string json = JsonConvert.SerializeObject(value);
            IDatabase cache = _multiplexer.Value.GetDatabase();
            await cache.StringSetAsync(key, json, lifeTime);
        }

        public async Task Set(string key, object value, DateTime expiresAtUtc)
        {
            string json = JsonConvert.SerializeObject(value);
            IDatabase cache = _multiplexer.Value.GetDatabase();
            await cache.StringSetAsync(key, json, expiresAtUtc.Subtract(DateTime.UtcNow));
        }

        public async Task<T> Get<T>(string key)
        {
            IDatabase cache = _multiplexer.Value.GetDatabase();
            string json = await cache.StringGetAsync(key);
            if (json == null)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
