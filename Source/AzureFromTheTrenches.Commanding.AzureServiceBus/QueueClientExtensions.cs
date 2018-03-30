using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureServiceBus.Implementation;
using Microsoft.Azure.ServiceBus;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus
{
    public static class QueueClientExtensions
    {
        public static ICommandDispatcher CreateCommandDispatcher(
            this QueueClient queueClient,
            IServiceBusMessageSerializer serializer = null)
        {
            return new CommandQueueDispatcher(queueClient, serializer ?? new JsonServiceBusMessageSerializer());
        }

        public static Func<ICommandDispatcher> CreateCommandDispatcherFactory(
            this QueueClient queueClient,
            IServiceBusMessageSerializer serializer = null)
        {
            return () => new CommandQueueDispatcher(queueClient, serializer ?? new JsonServiceBusMessageSerializer());
        }
    }
}
