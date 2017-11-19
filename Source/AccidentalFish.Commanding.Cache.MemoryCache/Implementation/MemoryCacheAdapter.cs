using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace AccidentalFish.Commanding.Cache.MemoryCache.Implementation
{
    class MemoryCacheAdapter : ICacheAdapter
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheAdapter(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task Set(string key, object value, TimeSpan lifeTime)
        {
            _memoryCache.Set(key, value, lifeTime);
            return Task.FromResult(0);
        }

        public Task Set(string key, object value, DateTime expiresAt)
        {
            _memoryCache.Set(key, value, expiresAt);
            return Task.FromResult(0);
        }

        public Task<T> Get<T>(string key)
        {
            if (!_memoryCache.TryGetValue(key, out T result))
            {
                result = default(T);
            }
            return Task.FromResult(result);
        }
    }
}
