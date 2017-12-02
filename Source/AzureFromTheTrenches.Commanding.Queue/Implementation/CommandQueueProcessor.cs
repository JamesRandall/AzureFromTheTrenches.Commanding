using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Queue.Model;

namespace AzureFromTheTrenches.Commanding.Queue.Implementation
{
    internal class CommandQueueProcessor : ICommandQueueProcessor
    {
        private readonly IDirectCommandExecuter _commandExecuter;

        public CommandQueueProcessor(IDirectCommandExecuter commandExecuter)
        {
            _commandExecuter = commandExecuter;
        }

        public Task<bool> DequeueErrorHandler(Exception ex)
        {
            return Task.FromResult(true);
        }

        public async Task<bool> HandleRecievedItemAsync<TCommand, TResult>(QueueItem<TCommand> item, int maxDequeueCount) where TCommand : class, ICommand<TResult>
        {
            try
            {
                bool shouldDequeue = true;
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
                return shouldDequeue;
            }
            catch (Exception)
            {
                if (item.DequeueCount > maxDequeueCount)
                {
                    return true;
                }
                return false;
            }

        }
    }
}
