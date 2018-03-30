using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Queue;
using Microsoft.Azure.ServiceBus;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus.Implementation
{
    class ServiceBusCommandQueueProcessorFactory : IServiceBusCommandQueueProcessorFactory
    {
        private readonly ICommandQueueProcessorLogger _logger;
        private readonly ICommandExecuter _commandExecuter;
        private readonly IServiceBusMessageSerializer _serviceBusMessageSerializer;

        public ServiceBusCommandQueueProcessorFactory(ICommandQueueProcessorLogger logger,
            ICommandExecuter commandExecuter,
            IServiceBusMessageSerializer serviceBusMessageSerializer)
        {
            _logger = logger;
            _commandExecuter = commandExecuter;
            _serviceBusMessageSerializer = serviceBusMessageSerializer;
        }

        public IServiceBusCommandQueueProcessor Create<TCommand, TResult>(QueueClient queueClient, int numberOfConcurrentListeners = 1,
            TimeSpan? maxAutoRenewDuration = null) where TCommand : ICommand<TResult>
        {
            return new ServiceBusCommandQueueProcessor<TCommand, TResult>(
                queueClient,
                _logger,
                _commandExecuter,
                _serviceBusMessageSerializer,
                numberOfConcurrentListeners,
                maxAutoRenewDuration);
        }

        public IServiceBusCommandQueueProcessor Create<TCommand>(QueueClient queueClient, int numberOfConcurrentListeners = 1,
            TimeSpan? maxAutoRenewDuration = null) where TCommand : ICommand
        {
            return new ServiceBusCommandQueueProcessor<TCommand>(
                queueClient,
                _logger,
                _commandExecuter,
                _serviceBusMessageSerializer,
                numberOfConcurrentListeners,
                maxAutoRenewDuration);
        }
    }
}
