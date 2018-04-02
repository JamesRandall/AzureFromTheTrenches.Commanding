using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Cache.MemoryCache.Implementation;

namespace AzureFromTheTrenches.Commanding.Cache.MemoryCache
{
    // ReSharper disable once InconsistentNaming
    public static class MemoryCacheCommandingDependencies
    {
        [Obsolete("Please use AddCommandMemoryCache instead")]
        public static ICommandingDependencyResolver UseCommandMemoryCache(this ICommandingDependencyResolver resolver)
        {
            resolver.TypeMapping<ICacheAdapter, MemoryCacheAdapter>();

            return resolver;
        }

        public static ICommandingDependencyResolverAdapter AddCommandMemoryCache(this ICommandingDependencyResolverAdapter resolver)
        {
            resolver.TypeMapping<ICacheAdapter, MemoryCacheAdapter>();

            return resolver;
        }
    }
}
