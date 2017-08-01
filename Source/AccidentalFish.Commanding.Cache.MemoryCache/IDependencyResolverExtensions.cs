using AccidentalFish.Commanding.Cache.MemoryCache.Implementation;
using AccidentalFish.DependencyResolver;

namespace AccidentalFish.Commanding.Cache.MemoryCache
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseCommandMemoryCache(this IDependencyResolver resolver)
        {
            resolver.Register<ICacheWrapper, MemoryCacheWrapperImpl>();

            return resolver;
        }
    }
}
