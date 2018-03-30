using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Azure.ServiceBus;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus
{
    public interface IServiceBusCommandQueueProcessorFactory
    {
        IServiceBusCommandQueueProcessor Create<TCommand, TResult>(
            QueueClient queueClient,
            int numberOfConcurrentListeneres=1,
            TimeSpan? maxAutoRenewDuration = null) where TCommand : ICommand<TResult>;

        IServiceBusCommandQueueProcessor Create<TCommand>(
            QueueClient queueClient,
            int numberOfConcurrentListeneres = 1,
            TimeSpan? maxAutoRenewDuration = null) where TCommand : ICommand;
    }
}
