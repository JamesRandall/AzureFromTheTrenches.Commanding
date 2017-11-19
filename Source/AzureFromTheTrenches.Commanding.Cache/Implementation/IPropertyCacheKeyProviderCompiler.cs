using System;

namespace AzureFromTheTrenches.Commanding.Cache.Implementation
{
    interface IPropertyCacheKeyProviderCompiler
    {
        Func<TCommand, string> Compile<TCommand>(ICacheKeyHash cacheKeyHash);
    }
}
