using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Queue.Model;

namespace AccidentalFish.Commanding.Queue.Implementation
{
    internal class CommandQueueProcessor : ICommandQueueProcessor
    {
        private readonly ICommandExecuter _commandExecuter;

        public CommandQueueProcessor(ICommandExecuter commandExecuter)
        {
            _commandExecuter = commandExecuter;
        }

        public Task<bool> DequeueErrorHandler(Exception ex)
        {
            return Task.FromResult(true);
        }

        public async Task<bool> HandleRecievedItemAsync<TCommand, TResult>(QueueItem<TCommand> item, int maxDequeueCount) where TCommand : class
        {
            try
            {
                bool shouldDequeue = true;
                IQueueableCommand queueableCommand = item.Item as IQueueableCommand;
                if (queueableCommand != null)
                {
                    queueableCommand.DequeueCount = item.DequeueCount;
                }
                Task commandTask = _commandExecuter.ExecuteAsync<TCommand, TResult>(item.Item);
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
