using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Cache.MemoryCache.Implementation;

namespace AccidentalFish.Commanding.Cache.MemoryCache
{
    // ReSharper disable once InconsistentNaming
    public static class MemoryCacheCommandingDependencies
    {
        public static ICommandingDependencyResolver UseCommandMemoryCache(this ICommandingDependencyResolver resolver)
        {
            resolver.TypeMapping<ICacheAdapter, MemoryCacheAdapter>();

            return resolver;
        }
    }
}
