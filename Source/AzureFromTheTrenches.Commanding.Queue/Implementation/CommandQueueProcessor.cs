using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Queue.Model;

namespace AzureFromTheTrenches.Commanding.Queue.Implementation
{
    internal class CommandQueueProcessor : ICommandQueueProcessor
    {
        private readonly IDirectCommandExecuter _commandExecuter;
        private readonly ICommandQueueProcessorLogger _logger;

        public CommandQueueProcessor(IDirectCommandExecuter commandExecuter,
            ICommandQueueProcessorLogger logger)
        {
            _commandExecuter = commandExecuter;
            _logger = logger;
        }

        public Task<bool> DequeueErrorHandler(Exception ex)
        {
            _logger.LogError($"Error during dequeue {ex.GetType().Name}", null, ex);
            return Task.FromResult(true);
        }

        public async Task<bool> HandleRecievedItemAsync<TCommand, TResult>(QueueItem<TCommand> item, int maxDequeueCount) where TCommand : class, ICommand<TResult>
        {
            try
            {
                _logger.LogInfo($"Recieved command {item.GetType().Name} from queue");
                bool shouldDequeue = true;
                // ReSharper disable once SuspiciousTypeConversion.Global - IQueueableCommand is implemented by package users
                IQueueableCommand queueableCommand = item.Item as IQueueableCommand;
                if (queueableCommand != null)
                {
                    queueableCommand.DequeueCount = item.DequeueCount;
                }
                Task commandTask = _commandExecuter.ExecuteAsync(item.Item);
                while (!commandTask.Wait(TimeSpan.FromSeconds(10)))
                {
                    await item.ExtendLeaseAsync();
                }

                if (queueableCommand != null)
                {
                    shouldDequeue = queueableCommand.ShouldDequeue;
                }
                _logger.LogInfo($"Completed processing command {item.GetType().Name} and returning a shouldDequeue status of {shouldDequeue}");
                return shouldDequeue;
            }
            catch (Exception ex)
            {
                if (item.DequeueCount > maxDequeueCount)
                {
                    _logger.LogError($"Error during processing command of type {item.GetType().Name} with exception of type {ex.GetType().AssemblyQualifiedName}. Dequeue count is {item.DequeueCount}, will not try again.", item.Item, ex);
                    return true;
                }
                _logger.LogWarning($"Error during processing command of type {item.GetType().Name} with exception of type {ex.GetType().AssemblyQualifiedName}. Dequeue count is {item.DequeueCount}, will try again.", item.Item, ex);
                return false;
            }
        }

        public async Task<bool> HandleRecievedItemAsync<TCommand>(QueueItem<TCommand> item, int maxDequeueCount) where TCommand : class, ICommand
        {
            try
            {
                _logger.LogInfo($"Recieved command {item.GetType().Name} from queue");
                bool shouldDequeue = true;
                // ReSharper disable once SuspiciousTypeConversion.Global - IQueueableCommand is implemented by package users
                IQueueableCommand queueableCommand = item.Item as IQueueableCommand;
                if (queueableCommand != null)
                {
                    queueableCommand.DequeueCount = item.DequeueCount;
                }
                NoResultCommandWrapper wrappedCommand = new NoResultCommandWrapper(item.Item);
                Task commandTask = _commandExecuter.ExecuteAsync(wrappedCommand);
                while (!commandTask.Wait(TimeSpan.FromSeconds(10)))
                {
                    await item.ExtendLeaseAsync();
                }

                if (queueableCommand != null)
                {
                    shouldDequeue = queueableCommand.ShouldDequeue;
                }
                _logger.LogInfo($"Completed processing command {item.GetType().Name} and returning a shouldDequeue status of {shouldDequeue}");
                return shouldDequeue;
            }
            catch (Exception ex)
            {
                if (item.DequeueCount > maxDequeueCount)
                {
                    _logger.LogError($"Error during processing command of type {item.GetType().Name} with exception of type {ex.GetType().AssemblyQualifiedName}. Dequeue count is {item.DequeueCount}, will not try again.", item.Item, ex);
                    return true;
                }
                _logger.LogWarning($"Error during processing command of type {item.GetType().Name} with exception of type {ex.GetType().AssemblyQualifiedName}. Dequeue count is {item.DequeueCount}, will try again.", item.Item, ex);
                return false;
            }
        }
    }
}
