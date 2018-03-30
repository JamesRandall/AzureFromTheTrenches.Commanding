using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Queue;
using Microsoft.Azure.ServiceBus;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus.Implementation
{
    internal class ServiceBusCommandQueueProcessor<TCommand, TResult> : AbstractServiceBusCommandQueueProcessor<TCommand> where TCommand : ICommand<TResult>
    {
        public ServiceBusCommandQueueProcessor(QueueClient queueClient,
            ICommandQueueProcessorLogger logger,
            ICommandExecuter commandExecuter,
            IServiceBusMessageSerializer serializer,
            int numberOfConcurrentListeners,
            TimeSpan? maxAutoRenewDuration) : base(queueClient, logger, commandExecuter, serializer, numberOfConcurrentListeners, maxAutoRenewDuration)
        {
            
        }

        protected override Task ExecuteCommandAsync(TCommand command, CancellationToken cancellationToken)
        {
            ICommand<TResult> castCommand = (ICommand<TResult>) command;
            return CommandExecuter.ExecuteAsync(castCommand, cancellationToken);
        }
    }

    internal class ServiceBusCommandQueueProcessor<TCommand> : AbstractServiceBusCommandQueueProcessor<TCommand> where TCommand : ICommand
    {
        public ServiceBusCommandQueueProcessor(QueueClient queueClient,
            ICommandQueueProcessorLogger logger,
            ICommandExecuter commandExecuter,
            IServiceBusMessageSerializer serializer,
            int numberOfConcurrentListeners,
            TimeSpan? maxAutoRenewDuration) : base(queueClient, logger, commandExecuter, serializer, numberOfConcurrentListeners, maxAutoRenewDuration)
        {

        }

        protected override Task ExecuteCommandAsync(TCommand command, CancellationToken cancellationToken)
        {
            NoResultCommandWrapper wrapper = new NoResultCommandWrapper(command);
            return CommandExecuter.ExecuteAsync(wrapper, cancellationToken);
        }
    }
}
