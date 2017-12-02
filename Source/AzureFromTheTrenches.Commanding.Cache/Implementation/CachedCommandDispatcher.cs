using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Cache.Implementation
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

        public async Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
        {
            CacheOptions options = _cacheOptionsProvider.Get(command);
            if (options == null)
            {
                return await _commandDispatcher.DispatchAsync(command, cancellationToken);
            }

            var cacheKey = CacheKey(command);

            TResult result = await _cacheAdapter.Get<TResult>(cacheKey);
            if (result != null)
            {
                return new CommandResult<TResult>(result, false);
            }

            CommandResult<TResult> executedResult;
            if (options.Semaphore != null)
            {
                await options.Semaphore.WaitAsync(cancellationToken);
                try
                {
                    result = await _cacheAdapter.Get<TResult>(cacheKey);
                    if (result != null)
                    {
                        return new CommandResult<TResult>(result, false);
                    }
                    else
                    {
                        executedResult = await _commandDispatcher.DispatchAsync(command, cancellationToken);
                    }
                }
                finally
                {
                    options.Semaphore.Release();
                }
            }
            else
            {
                executedResult = await _commandDispatcher.DispatchAsync(command, cancellationToken);
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

        public Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken)
        {
            // we don't cache the results of commands with no results! - just pass it on
            return _commandDispatcher.DispatchAsync(command, cancellationToken);
        }

        public ICommandExecuter AssociatedExecuter => _commandDispatcher.AssociatedExecuter;

        private string CacheKey<TCommand>(TCommand command) where TCommand : class
        {
            if (!(command is ICacheKeyProvider keyProvider))
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

