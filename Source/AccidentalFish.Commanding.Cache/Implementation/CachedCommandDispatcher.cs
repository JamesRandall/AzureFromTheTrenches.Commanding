using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;

namespace AccidentalFish.Commanding.Cache.Implementation
{
    internal class CachedCommandDispatcher : ICachedCommandDispatcher
    {
        private readonly ICacheKeyProvider _cacheKeyProvider;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ICacheOptionsProvider _cacheOptionsProvider;
        private readonly ICacheAdapter _cacheAdapter;

        public CachedCommandDispatcher(ICacheKeyProvider cacheKeyProvider,
            ICommandDispatcher commandDispatcher,
            ICacheOptionsProvider cacheOptionsProvider,
            ICacheAdapter cacheAdapter)
        {
            _cacheKeyProvider = cacheKeyProvider;
            _commandDispatcher = commandDispatcher;
            _cacheOptionsProvider = cacheOptionsProvider;
            _cacheAdapter = cacheAdapter;
        }

        public async Task<CommandResult<TResult>> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : class
        {
            CacheOptions options = _cacheOptionsProvider.Get(command);
            if (options == null)
            {
                return await _commandDispatcher.DispatchAsync<TCommand, TResult>(command);
            }

            var cacheKey = CacheKey(command);

            TResult result = await _cacheAdapter.Get<TResult>(cacheKey);
            if (result != null)
            {
                return new CommandResult<TResult>(result, false);
            }

            CommandResult<TResult> executedResult = null;
            if (options.Semaphore != null)
            {
                await options.Semaphore.WaitAsync();
                try
                {
                    result = await _cacheAdapter.Get<TResult>(cacheKey);
                    if (result != null)
                    {
                        return new CommandResult<TResult>(result, false);
                    }
                    else
                    {
                        executedResult = await _commandDispatcher.DispatchAsync<TCommand, TResult>(command);
                    }
                }
                finally
                {
                    options.Semaphore.Release();
                }
            }
            else
            {
                executedResult = await _commandDispatcher.DispatchAsync<TCommand, TResult>(command);
            }

            if (options.LifeTime != null)
            {
                await _cacheAdapter.Set(cacheKey, executedResult.Result, options.LifeTime());
            }
            else if (options.ExpiresAtUtc != null)
            {
                await _cacheAdapter.Set(cacheKey, executedResult.Result, options.ExpiresAtUtc());
            }
            else
            {
                // shouldn't happen but lets make sure we spit out a sensible error if it does
                throw new CacheConfigurationException("Either a lifetime or expiry date must be set for a cached command");
            }
            
            return executedResult;
        }

        public Task<CommandResult<NoResult>> DispatchAsync<TCommand>(TCommand command) where TCommand : class
        {
            CacheOptions options = _cacheOptionsProvider.Get(command);
            if (options != null)
            {
                throw new CacheConfigurationException($"Results cannot be cached for an execution chain that produces no results. Command type {typeof(TCommand)}");
            }

            return  _commandDispatcher.DispatchAsync(command);
        }

        public Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command)
        {
            throw new NotImplementedException();
        }

        public ICommandExecuter AssociatedExecuter => _commandDispatcher.AssociatedExecuter;

        private string CacheKey<TCommand>(TCommand command) where TCommand : class
        {
            ICacheKeyProvider keyProvider = command as ICacheKeyProvider;
            if (keyProvider == null)
            {
                keyProvider = _cacheKeyProvider;
            }
            string cacheKey = keyProvider.CacheKey(command);

            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                throw new CacheKeyException($"Cache key cannot be null or whitespace for command {typeof(TCommand)}");
            }
            return cacheKey;
        }
    }
}

