using System;
using AccidentalFish.Commanding.Cache.Redis.Implementation;
using AccidentalFish.DependencyResolver;
using StackExchange.Redis;

namespace AccidentalFish.Commanding.Cache.Redis
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseCommandRedisCache(this IDependencyResolver resolver, string connectionString)
        {
            ICacheAdapter adapter = new RedisCacheAdapter(new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString)));
            resolver.RegisterInstance(adapter);
            return resolver;
        }

        public static IDependencyResolver UseCommandRedisCache(this IDependencyResolver resolver, ConnectionMultiplexer multiplexer)
        {
            ICacheAdapter adapter = new RedisCacheAdapter(new Lazy<ConnectionMultiplexer>(() => multiplexer));
            resolver.RegisterInstance(adapter);
            return resolver;
        }

        public static IDependencyResolver UseCommandRedisCache(this IDependencyResolver resolver, Lazy<ConnectionMultiplexer> multiplexer)
        {
            ICacheAdapter adapter = new RedisCacheAdapter(multiplexer);
            resolver.RegisterInstance(adapter);
            return resolver;
        }
    }
}
