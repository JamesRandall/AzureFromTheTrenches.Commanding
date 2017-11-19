using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureFromTheTrenches.Commanding.Cache.Implementation
{
    internal class CacheOptionsProvider : ICacheOptionsProvider
    {
        private readonly Dictionary<Type, CacheOptions> _options;

        private readonly List<EvalCacheOptions> _evalCacheOptions;

        public CacheOptionsProvider(IEnumerable<CacheOptions> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var cacheOptions = options as CacheOptions[] ?? options.ToArray();
            _options = cacheOptions.Where(x => !(x is EvalCacheOptions)).ToDictionary(x => x.CommandType, x => x);
            _evalCacheOptions = cacheOptions.Select(x => x as EvalCacheOptions).Where(x => x != null).ToList();
        }

        public CacheOptions Get<T>(T command)
        {
            CacheOptions result;
            if (_options.TryGetValue(command.GetType(), out result))
            {
                return result;
            }
            foreach (EvalCacheOptions options in _evalCacheOptions)
            {
                if (options.IsCachedCommand(command))
                {
                    return options;
                }
            }
            return null;
        }
    }
}
