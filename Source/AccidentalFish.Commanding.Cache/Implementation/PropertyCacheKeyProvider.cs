using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace AccidentalFish.Commanding.Cache.Implementation
{
    class PropertyCacheKeyProvider : ICacheKeyProvider
    {
        private readonly ConcurrentDictionary<Type, Func<object, string>> _cachedKeyProviders = new ConcurrentDictionary<Type, Func<object, string>>();
        private readonly IPropertyCacheKeyProviderCompiler _compiler;

        public PropertyCacheKeyProvider(IPropertyCacheKeyProviderCompiler compiler)
        {
            _compiler = compiler;
        }

        public string CacheKey<TCommand>(TCommand command)
        {
            Type commandType = command.GetType();
            if (!_cachedKeyProviders.TryGetValue(command.GetType(), out Func<object, string> func))
            {
                Func<TCommand, string> commandFunc = _compiler.Compile<TCommand>();
                func = (o) => commandFunc((TCommand) o);
                _cachedKeyProviders[commandType] = func;
            }
            return func(command);
        }
    }
}
