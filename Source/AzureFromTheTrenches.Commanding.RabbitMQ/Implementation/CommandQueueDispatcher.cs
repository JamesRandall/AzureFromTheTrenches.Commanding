using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using System.Threading;
using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.RabbitMQ.Implementation
{
    class CommandQueueDispatcher : ICommandDispatcher
    {
        private readonly IQueueClient _queueClient;
        private readonly IRabbitMQMessageSerializer _serializer;

        public CommandQueueDispatcher(IQueueClient queueClient,
            IRabbitMQMessageSerializer serializer)
        {
            _queueClient = queueClient;
            _serializer = serializer;
        }

        public async Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] body = _serializer.Serialize(command);
            await _queueClient.SendAsync(body);
            return new CommandResult<TResult>(default(TResult), true);
        }

        public async Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] messageBytes = _serializer.Serialize(command);
            await _queueClient.SendAsync(messageBytes);
            return new CommandResult(true);
        }

        public ICommandExecuter AssociatedExecuter { get; }
    }
}
