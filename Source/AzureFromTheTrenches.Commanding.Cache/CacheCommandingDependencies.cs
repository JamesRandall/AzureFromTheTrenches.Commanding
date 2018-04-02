using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Cache.Implementation;

namespace AzureFromTheTrenches.Commanding.Cache
{
    // ReSharper disable once InconsistentNaming
    public static class CacheCommandingDependencies
    {
        /// <summary>
        /// Sets up the cache with the default cache key provider that uses the command type, property names and property values to
        /// generate a hashable string
        /// </summary>
        /// <param name="resolver">The dependency resolver</param>
        /// <param name="options">Cache options</param>
        /// <returns>The dependency resolver</returns>
        [Obsolete("Please use AddCommandCache instead")]
        public static ICommandingDependencyResolver UseCommandCache(this ICommandingDependencyResolver resolver,  params CacheOptions[] options)
        {
            return UseCommandCache(resolver, new PropertyCacheKeyProvider(new PropertyCacheKeyProviderCompiler(), new SimpleCacheKeyHash()), options);
        }

        /// <summary>
        /// Sets up the cache with the default cache key provider that uses the command type, property names and property values to
        /// generate a hashable string
        /// </summary>
        /// <param name="resolver">The dependency resolver</param>
        /// <param name="options">Cache options</param>
        /// <returns>The dependency resolver</returns>
        public static ICommandingDependencyResolverAdapter AddCommandCache(this ICommandingDependencyResolverAdapter resolver, params CacheOptions[] options)
        {
            return AddCommandCache(resolver, new PropertyCacheKeyProvider(new PropertyCacheKeyProviderCompiler(), new SimpleCacheKeyHash()), options);
        }

        /// <summary>
        /// Sets up the cache with the default cache key provider that uses the command type, property names and property values to
        /// generate a hashable string
        /// </summary>
        /// <param name="resolver">The dependency resolver</param>
        /// <param name="options">Cache options</param>
        /// <param name="replaceDefaultCommandDispatcher">If true then the default ICommandDispatcher will be replaced with the caching variant</param>
        /// <returns>The dependency resolver</returns>
        [Obsolete("Please use AddCommandCache instead")]
        public static ICommandingDependencyResolver UseCommandCache(this ICommandingDependencyResolver resolver, bool replaceDefaultCommandDispatcher, params CacheOptions[] options)
        {
            return UseCommandCache(resolver, new PropertyCacheKeyProvider(new PropertyCacheKeyProviderCompiler(), new SimpleCacheKeyHash()), replaceDefaultCommandDispatcher, options);
        }

        /// <summary>
        /// Sets up the cache with the default cache key provider that uses the command type, property names and property values to
        /// generate a hashable string
        /// </summary>
        /// <param name="resolver">The dependency resolver</param>
        /// <param name="options">Cache options</param>
        /// <param name="replaceDefaultCommandDispatcher">If true then the default ICommandDispatcher will be replaced with the caching variant</param>
        /// <returns>The dependency resolver</returns>
        public static ICommandingDependencyResolverAdapter AddCommandCache(this ICommandingDependencyResolverAdapter resolver, bool replaceDefaultCommandDispatcher, params CacheOptions[] options)
        {
            return AddCommandCache(resolver, new PropertyCacheKeyProvider(new PropertyCacheKeyProviderCompiler(), new SimpleCacheKeyHash()), replaceDefaultCommandDispatcher, options);
        }

        /// <summary>
        /// Sets up the cache with the specified cache key provider
        /// </summary>
        /// <param name="resolver">The dependency resolver</param>
        /// <param name="cacheKeyProvider">Instance of a cache key provider</param>
        /// <param name="options">Cache options</param>
        /// <returns>The dependency resolver</returns>
        [Obsolete("Please use AddCommandCache instead")]
        public static ICommandingDependencyResolver UseCommandCache(this ICommandingDependencyResolver resolver, ICacheKeyProvider cacheKeyProvider, params CacheOptions[] options)
        {
            return UseCommandCache(resolver, cacheKeyProvider, false, options);            
        }

        /// <summary>
        /// Sets up the cache with the specified cache key provider
        /// </summary>
        /// <param name="resolver">The dependency resolver</param>
        /// <param name="cacheKeyProvider">Instance of a cache key provider</param>
        /// <param name="options">Cache options</param>
        /// <returns>The dependency resolver</returns>
        public static ICommandingDependencyResolverAdapter AddCommandCache(this ICommandingDependencyResolverAdapter resolver, ICacheKeyProvider cacheKeyProvider, params CacheOptions[] options)
        {
            return AddCommandCache(resolver, cacheKeyProvider, false, options);
        }

        /// <summary>
        /// Sets up the cache with the specified cache key provider
        /// </summary>
        /// <param name="resolver">The dependency resolver</param>
        /// <param name="cacheKeyProvider">Instance of a cache key provider</param>
        /// <param name="replaceDefaultCommandDispatcher">If true then the default ICommandDispatcher will be replaced with the caching variant</param>
        /// <param name="options">Cache options</param>
        /// <returns>The dependency resolver</returns>
        [Obsolete("Please use AddCommandCache instead")]
        public static ICommandingDependencyResolver UseCommandCache(
            this ICommandingDependencyResolver resolver,
            ICacheKeyProvider cacheKeyProvider,
            bool replaceDefaultCommandDispatcher,
            params CacheOptions[] options)
        {
            ICacheOptionsProvider cacheOptionsProvider = new CacheOptionsProvider(options);
            resolver.RegisterInstance(cacheOptionsProvider);
            if (replaceDefaultCommandDispatcher)
            {
                resolver.TypeMapping<ICommandDispatcher, CachedCommandDispatcher>();
            }
            else
            {
                resolver.TypeMapping<ICachedCommandDispatcher, CachedCommandDispatcher>();
            }
            resolver.RegisterInstance(cacheKeyProvider);

            return resolver;
        }

        /// <summary>
        /// Sets up the cache with the specified cache key provider
        /// </summary>
        /// <param name="resolver">The dependency resolver</param>
        /// <param name="cacheKeyProvider">Instance of a cache key provider</param>
        /// <param name="replaceDefaultCommandDispatcher">If true then the default ICommandDispatcher will be replaced with the caching variant</param>
        /// <param name="options">Cache options</param>
        /// <returns>The dependency resolver</returns>
        public static ICommandingDependencyResolverAdapter AddCommandCache(
            this ICommandingDependencyResolverAdapter resolver,
            ICacheKeyProvider cacheKeyProvider,
            bool replaceDefaultCommandDispatcher,
            params CacheOptions[] options)
        {
            ICacheOptionsProvider cacheOptionsProvider = new CacheOptionsProvider(options);
            resolver.RegisterInstance(cacheOptionsProvider);
            if (replaceDefaultCommandDispatcher)
            {
                resolver.TypeMapping<ICommandDispatcher, CachedCommandDispatcher>();
            }
            else
            {
                resolver.TypeMapping<ICachedCommandDispatcher, CachedCommandDispatcher>();
            }
            resolver.RegisterInstance(cacheKeyProvider);

            return resolver;
        }
    }
}
