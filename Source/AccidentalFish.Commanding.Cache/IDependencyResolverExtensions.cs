using AccidentalFish.Commanding.Cache.Implementation;
using AccidentalFish.DependencyResolver;

namespace AccidentalFish.Commanding.Cache
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        /// <summary>
        /// Sets up the cache with the default cache key provider that uses the command type, property names and property values to
        /// generate a hashable string
        /// </summary>
        /// <param name="resolver">The dependency resolver</param>
        /// <param name="options">Cache options</param>
        /// <returns>The dependency resolver</returns>
        public static IDependencyResolver UseCommandCache(this IDependencyResolver resolver,  params CacheOptions[] options)
        {
            return UseCommandCache(resolver, new PropertyCacheKeyProvider(new PropertyCacheKeyProviderCompiler()), options);
        }

        /// <summary>
        /// Sets up the cache with the specified cache key provider
        /// </summary>
        /// <param name="resolver">The dependency resolver</param>
        /// <param name="cacheKeyProvider">Instance of a cache key provider</param>
        /// <param name="options">Cache options</param>
        /// <returns>The dependency resolver</returns>
        public static IDependencyResolver UseCommandCache(this IDependencyResolver resolver, ICacheKeyProvider cacheKeyProvider, params CacheOptions[] options)
        {
            ICacheOptionsProvider cacheOptionsProvider = new CacheOptionsProvider(options);
            resolver.RegisterInstance(cacheOptionsProvider);
            resolver.Register<ICachedCommandDispatcher, CachedCommandDispatcher>();
            resolver.RegisterInstance(cacheKeyProvider);

            return resolver;
        }
    }
}
