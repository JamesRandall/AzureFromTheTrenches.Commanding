using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Cache.Redis.Implementation;
using StackExchange.Redis;

namespace AzureFromTheTrenches.Commanding.Cache.Redis
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        [Obsolete("Please use AddCommandRedisCache instead")]
        public static ICommandingDependencyResolver UseCommandRedisCache(this ICommandingDependencyResolver resolver, string connectionString)
        {
            ICacheAdapter adapter = new RedisCacheAdapter(new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString)));
            resolver.RegisterInstance(adapter);
            return resolver;
        }

        [Obsolete("Please use AddCommandRedisCache instead")]
        public static ICommandingDependencyResolver UseCommandRedisCache(this ICommandingDependencyResolver resolver, ConnectionMultiplexer multiplexer)
        {
            ICacheAdapter adapter = new RedisCacheAdapter(new Lazy<ConnectionMultiplexer>(() => multiplexer));
            resolver.RegisterInstance(adapter);
            return resolver;
        }

        [Obsolete("Please use AddCommandRedisCache instead")]
        public static ICommandingDependencyResolver UseCommandRedisCache(this ICommandingDependencyResolver resolver, Lazy<ConnectionMultiplexer> multiplexer)
        {
            ICacheAdapter adapter = new RedisCacheAdapter(multiplexer);
            resolver.RegisterInstance(adapter);
            return resolver;
        }

        public static ICommandingDependencyResolverAdapter AddCommandRedisCache(this ICommandingDependencyResolverAdapter resolver, string connectionString)
        {
            ICacheAdapter adapter = new RedisCacheAdapter(new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString)));
            resolver.RegisterInstance(adapter);
            return resolver;
        }

        public static ICommandingDependencyResolverAdapter AddCommandRedisCache(this ICommandingDependencyResolverAdapter resolver, ConnectionMultiplexer multiplexer)
        {
            ICacheAdapter adapter = new RedisCacheAdapter(new Lazy<ConnectionMultiplexer>(() => multiplexer));
            resolver.RegisterInstance(adapter);
            return resolver;
        }

        public static ICommandingDependencyResolverAdapter AddCommandRedisCache(this ICommandingDependencyResolverAdapter resolver, Lazy<ConnectionMultiplexer> multiplexer)
        {
            ICacheAdapter adapter = new RedisCacheAdapter(multiplexer);
            resolver.RegisterInstance(adapter);
            return resolver;
        }
    }
}
