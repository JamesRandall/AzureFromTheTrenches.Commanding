using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using Microsoft.Azure.ServiceBus;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus.Implementation
{
    class CommandQueueDispatcher : ICommandDispatcher
    {
        private readonly QueueClient _queueClient;
        private readonly IServiceBusMessageSerializer _serializer;

        public CommandQueueDispatcher(QueueClient queueClient,
            IServiceBusMessageSerializer serializer)
        {
            _queueClient = queueClient;
            _serializer = serializer;
        }

        public async Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] messageBytes = _serializer.Serialize(command);
            Message message = new Message(messageBytes);
            await _queueClient.SendAsync(message);
            return new CommandResult<TResult>(default(TResult), true);
        }

        public async Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] messageBytes= _serializer.Serialize(command);
            Message message = new Message(messageBytes);
            message.SessionId = GetSessionId(command);
            await _queueClient.SendAsync(message);
            return new CommandResult(true);
        }

        public ICommandExecuter AssociatedExecuter { get; }

        protected virtual string GetSessionId(ICommand command)
        {
            return null;
        }
    }

    class CommandQueueDispatcher<TCommand> : CommandQueueDispatcher where TCommand : ICommand
    {
        private readonly Func<TCommand, string> _sessionIdProvider;

        public CommandQueueDispatcher(QueueClient queueClient, IServiceBusMessageSerializer serializer, Func<TCommand, string> sessionIdProvider) : base(queueClient, serializer)
        {
            _sessionIdProvider = sessionIdProvider;
        }

        protected override string GetSessionId(ICommand command)
        {
            if (_sessionIdProvider == null)
            {
                return null;
            }

            string sessionId = _sessionIdProvider((TCommand) command);
            return sessionId;
        }
    }
}
