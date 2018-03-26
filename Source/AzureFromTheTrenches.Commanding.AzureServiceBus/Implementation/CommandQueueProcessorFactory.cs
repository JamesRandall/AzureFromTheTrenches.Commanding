using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Queue;
using Microsoft.Azure.ServiceBus;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus.Implementation
{
    class CommandQueueProcessorFactory : ICommandQueueProcessorFactory
    {
        private readonly ICommandQueueProcessorLogger _logger;
        private readonly ICommandExecuter _commandExecuter;
        private readonly IServiceBusMessageSerializer _serviceBusMessageSerializer;

        public CommandQueueProcessorFactory(ICommandQueueProcessorLogger logger,
            ICommandExecuter commandExecuter,
            IServiceBusMessageSerializer serviceBusMessageSerializer)
        {
            _logger = logger;
            _commandExecuter = commandExecuter;
            _serviceBusMessageSerializer = serviceBusMessageSerializer;
        }

        public ICommandQueueProcessor Create<TCommand, TResult>(QueueClient queueClient, int numberOfConcurrentListeners = 1,
            TimeSpan? maxAutoRenewDuration = null) where TCommand : ICommand<TResult>
        {
            return new CommandQueueProcessor<TCommand, TResult>(
                queueClient,
                _logger,
                _commandExecuter,
                _serviceBusMessageSerializer,
                numberOfConcurrentListeners,
                maxAutoRenewDuration);
        }

        public ICommandQueueProcessor Create<TCommand>(QueueClient queueClient, int numberOfConcurrentListeners = 1,
            TimeSpan? maxAutoRenewDuration = null) where TCommand : ICommand
        {
            return new CommandQueueProcessor<TCommand>(
                queueClient,
                _logger,
                _commandExecuter,
                _serviceBusMessageSerializer,
                numberOfConcurrentListeners,
                maxAutoRenewDuration);
        }
    }
}
