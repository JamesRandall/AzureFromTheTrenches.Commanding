using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    class AzureStorageQueueDispatcher : ICommandDispatcher
    {
        private readonly CloudQueue _queue;
        private readonly IAzureStorageQueueSerializer _serializer;

        public AzureStorageQueueDispatcher(CloudQueue queue, IAzureStorageQueueSerializer serializer)
        {
            _queue = queue;
            _serializer = serializer;
        }

        public async Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command)
        {
            string serializedCommand = _serializer.Serialize(command);
            await _queue.AddMessageAsync(new CloudQueueMessage(serializedCommand));

            return new CommandResult<TResult>(default(TResult), true);
        }

        public async Task<CommandResult> DispatchAsync(ICommand command)
        {
            string serializedCommand = _serializer.Serialize(command);
            await _queue.AddMessageAsync(new CloudQueueMessage(serializedCommand));

            return new CommandResult(true);
        }

        public ICommandExecuter AssociatedExecuter => null;
    }
}
