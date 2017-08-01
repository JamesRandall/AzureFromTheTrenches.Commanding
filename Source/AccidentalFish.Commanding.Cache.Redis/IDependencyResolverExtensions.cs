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
            ICacheWrapper wrapper = new RedisCacheWrapperImpl(new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString)));
            resolver.RegisterInstance(wrapper);
            return resolver;
        }

        public static IDependencyResolver UseCommandRedisCache(this IDependencyResolver resolver, ConnectionMultiplexer multiplexer)
        {
            ICacheWrapper wrapper = new RedisCacheWrapperImpl(new Lazy<ConnectionMultiplexer>(() => multiplexer));
            resolver.RegisterInstance(wrapper);
            return resolver;
        }

        public static IDependencyResolver UseCommandRedisCache(this IDependencyResolver resolver, Lazy<ConnectionMultiplexer> multiplexer)
        {
            ICacheWrapper wrapper = new RedisCacheWrapperImpl(multiplexer);
            resolver.RegisterInstance(wrapper);
            return resolver;
        }
    }
}
