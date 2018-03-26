using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using Microsoft.Azure.ServiceBus;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus.Implementation
{
    internal class CommandTopicDispatcher : ICommandDispatcher
    {
        private readonly TopicClient _topicClient;
        private readonly IServiceBusMessageSerializer _serializer;

        public CommandTopicDispatcher(TopicClient topicClient,
            IServiceBusMessageSerializer serializer)
        {
            _topicClient = topicClient;
            _serializer = serializer;
        }

        public async Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] messageBytes = _serializer.Serialize(command);
            Message message = new Message(messageBytes);
            await _topicClient.SendAsync(message);
            return new CommandResult<TResult>(default(TResult), true);
        }

        public async Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] messageBytes = _serializer.Serialize(command);
            Message message = new Message(messageBytes);
            await _topicClient.SendAsync(message);
            return new CommandResult(true);
        }

        public ICommandExecuter AssociatedExecuter { get; }
    }
}
