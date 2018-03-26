using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Azure.ServiceBus;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus
{
    public interface ICommandQueueProcessorFactory
    {
        ICommandQueueProcessor Create<TCommand, TResult>(
            QueueClient queueClient,
            int numberOfConcurrentListeneres=1,
            TimeSpan? maxAutoRenewDuration = null) where TCommand : ICommand<TResult>;
    }
}
