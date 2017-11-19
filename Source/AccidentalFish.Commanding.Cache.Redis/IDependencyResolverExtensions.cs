using System;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Cache.Redis.Implementation;
using StackExchange.Redis;

namespace AccidentalFish.Commanding.Cache.Redis
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static ICommandingDependencyResolver UseCommandRedisCache(this ICommandingDependencyResolver resolver, string connectionString)
        {
            ICacheAdapter adapter = new RedisCacheAdapter(new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString)));
            resolver.RegisterInstance(adapter);
            return resolver;
        }

        public static ICommandingDependencyResolver UseCommandRedisCache(this ICommandingDependencyResolver resolver, ConnectionMultiplexer multiplexer)
        {
            ICacheAdapter adapter = new RedisCacheAdapter(new Lazy<ConnectionMultiplexer>(() => multiplexer));
            resolver.RegisterInstance(adapter);
            return resolver;
        }

        public static ICommandingDependencyResolver UseCommandRedisCache(this ICommandingDependencyResolver resolver, Lazy<ConnectionMultiplexer> multiplexer)
        {
            ICacheAdapter adapter = new RedisCacheAdapter(multiplexer);
            resolver.RegisterInstance(adapter);
            return resolver;
        }
    }
}
