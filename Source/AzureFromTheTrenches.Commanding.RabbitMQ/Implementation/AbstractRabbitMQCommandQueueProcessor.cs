using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Queue;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.RabbitMQ.Implementation
{
    internal abstract class AbstractRabbitMQCommandQueueProcessor<TCommand> : IRabbitMQCommandQueueProcessor where TCommand : ICommand
    {
        private readonly QueueClient _queueClient;
        private readonly ICommandQueueProcessorLogger _logger;
        private readonly IRabbitMQMessageSerializer _serializer;

        protected ICommandExecuter CommandExecuter { get; }
        protected AbstractRabbitMQCommandQueueProcessor(QueueClient queueClient,
            ICommandQueueProcessorLogger logger,
            ICommandExecuter commandExecuter,
            IRabbitMQMessageSerializer serializer)
        {
            _logger = logger;
            _queueClient = queueClient;
            CommandExecuter = commandExecuter;
            _serializer = serializer;

            queueClient.RegisterHandler(ProcessMessageAsync);
            queueClient.RegisterConnectionRecoveryExceptionHandler(ConnectionRecoveryExceptionHandlerAsync);
            queueClient.RegisterConnectionExceptionHandler(ConnectionExceptionHandlerAsync);
            queueClient.RegisterChannelExceptionHandler(ChannelExceptionReceivedHandlerAsync);
            queueClient.RegisterChannelBasicRecoverOkHandler(ChannelBasicRecoverOkHandlerAsync);

            queueClient.StartConsuming();
        }

        protected abstract Task ExecuteCommandAsync(TCommand command, CancellationToken cancellationToken);

        private async Task ProcessMessageAsync(byte[] message, CancellationToken cancellationToken)
        {
            TCommand command = default(TCommand);
            try
            {
                command = _serializer.Deserialize<TCommand>(message);
                _logger.LogInfo($"Recieved command {command.GetType().Name} from queue");

                await ExecuteCommandAsync(command, cancellationToken);

                _logger.LogInfo($"Completed processing command {command.GetType().Name}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error during processing command of type {typeof(TCommand).Name} with exception of type {ex.GetType().AssemblyQualifiedName}.",
                    command, ex);
            }
        }

        private Task ConnectionRecoveryExceptionHandlerAsync(object sender, ConnectionRecoveryErrorEventArgs @event)
        {
            if (@event is null)
            {
                return null;
            }

            _logger.LogError(@event.Exception.Message, null, @event.Exception);
            return Task.CompletedTask;
        }

        private Task ConnectionExceptionHandlerAsync(object sender, CallbackExceptionEventArgs @event)
        {
            if (@event is null)
            {
                return null;
            }

            _logger.LogError(@event.Exception.Message, null, @event.Exception);
            return Task.CompletedTask;
        }

        private Task ChannelBasicRecoverOkHandlerAsync(object sender, EventArgs @event)
        {
            if (@event is null)
            {
                return null;
            }

            _logger.LogInfo("Connection has been reestablished.");
            return Task.CompletedTask;
        }

        private Task ChannelExceptionReceivedHandlerAsync(object sender, CallbackExceptionEventArgs @event)
        {
            if (@event is null)
            {
                return null;
            }

            _logger.LogError(@event.Exception.Message, null, @event.Exception);
            return Task.CompletedTask;
        }
    }
}
