using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Queue;
using Microsoft.Azure.ServiceBus;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus.Implementation
{
    internal abstract class AbstractServiceBusCommandQueueProcessor<TCommand> : IServiceBusCommandQueueProcessor where TCommand : ICommand
    {
        private readonly QueueClient _queueClient;
        private readonly ICommandQueueProcessorLogger _logger;
        private readonly IServiceBusMessageSerializer _serializer;

        protected AbstractServiceBusCommandQueueProcessor(QueueClient queueClient,
            ICommandQueueProcessorLogger logger,
            ICommandExecuter commandExecuter,
            IServiceBusMessageSerializer serializer,
            int numberOfConcurrentListeners,
            TimeSpan? maxAutoRenewDuration)
        {
            _queueClient = queueClient;
            _logger = logger;
            CommandExecuter = commandExecuter;
            _serializer = serializer;
            queueClient.RegisterMessageHandler(ProcessMessageAsync, new MessageHandlerOptions(ExceptionReceivedHandlerAsync)
            {
                AutoComplete = false,
                MaxConcurrentCalls = numberOfConcurrentListeners,
                MaxAutoRenewDuration = maxAutoRenewDuration ?? TimeSpan.FromHours(1)
            });
        }

        protected ICommandExecuter CommandExecuter { get; }

        private Task ExceptionReceivedHandlerAsync(ExceptionReceivedEventArgs arg)
        {
            _logger.LogError(arg.Exception.Message, null, arg.Exception);
            return Task.CompletedTask;
        }

        private async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
        {
            TCommand command = default(TCommand);
            try
            {
                command = _serializer.Deserialize<TCommand>(message.Body);
                _logger.LogInfo($"Recieved command {command.GetType().Name} from queue");
                bool shouldDequeue = true;

                // ReSharper disable once SuspiciousTypeConversion.Global
                IQueueableCommand queueableCommand = command as IQueueableCommand;
                if (queueableCommand != null)
                {
                    queueableCommand.DequeueCount = message.SystemProperties.DeliveryCount;
                }

                await ExecuteCommandAsync(command, cancellationToken);
                if (queueableCommand != null)
                {
                    shouldDequeue = queueableCommand.ShouldDequeue;
                }

                _logger.LogInfo($"Completed processing command {command.GetType().Name} and returning a shouldDequeue status of {shouldDequeue}");
                if (shouldDequeue)
                {
                    await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error during processing command of type {typeof(TCommand).Name} with exception of type {ex.GetType().AssemblyQualifiedName}. Dequeue count is {message.SystemProperties.DeliveryCount}, will try again.",
                    command, ex);
            }
        }

        protected abstract Task ExecuteCommandAsync(TCommand command, CancellationToken cancellationToken);
    }
}
