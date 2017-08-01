using AccidentalFish.Commanding.Cache.Implementation;
using AccidentalFish.DependencyResolver;

namespace AccidentalFish.Commanding.Cache
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseCommandCache(this IDependencyResolver resolver,  params CacheOptions[] options)
        {
            ICacheOptionsProvider cacheOptionsProvider = new CacheOptionsProvider(options);
            resolver.RegisterInstance(cacheOptionsProvider);
            resolver.Register<ICachedCommandDispatcher, CachedCommandDispatcher>();
            resolver.Register<ICacheKeyProvider, JsonCacheKeyProvider>();
            
            return resolver;
        }
    }
}
