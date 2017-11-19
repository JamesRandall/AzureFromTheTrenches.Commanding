using System;
using System.Collections.Concurrent;

namespace AzureFromTheTrenches.Commanding.Cache.Implementation
{
    class PropertyCacheKeyProvider : ICacheKeyProvider
    {
        private readonly ConcurrentDictionary<Type, Func<object, string>> _cachedKeyProviders = new ConcurrentDictionary<Type, Func<object, string>>();
        private readonly IPropertyCacheKeyProviderCompiler _compiler;
        private readonly ICacheKeyHash _cacheKeyHash;

        public PropertyCacheKeyProvider(IPropertyCacheKeyProviderCompiler compiler, ICacheKeyHash cacheKeyHash)
        {
            _compiler = compiler;
            _cacheKeyHash = cacheKeyHash;
        }

        public string CacheKey<TCommand>(TCommand command)
        {
            Type commandType = command.GetType();
            if (!_cachedKeyProviders.TryGetValue(command.GetType(), out Func<object, string> func))
            {
                Func<TCommand, string> commandFunc = _compiler.Compile<TCommand>(_cacheKeyHash);
                func = (o) => commandFunc((TCommand) o);
                _cachedKeyProviders[commandType] = func;
            }
            return func(command);
        }
    }
}
