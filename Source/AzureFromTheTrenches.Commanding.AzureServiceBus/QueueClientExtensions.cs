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
        
        public static ICommandDispatcher CreateCommandDispatcher<TCommand>(
            this QueueClient queueClient,
            IServiceBusMessageSerializer serializer = null,
            Func<TCommand, string> sessionIdProvider = null) where TCommand : ICommand
        {
            return new CommandQueueDispatcher<TCommand>(queueClient, serializer ?? new JsonServiceBusMessageSerializer(), sessionIdProvider);
        }

        public static Func<ICommandDispatcher> CreateCommandDispatcherFactory(
            this QueueClient queueClient,
            IServiceBusMessageSerializer serializer = null)
        {
            return () => new CommandQueueDispatcher(queueClient, serializer ?? new JsonServiceBusMessageSerializer());
        }
        
        public static Func<ICommandDispatcher> CreateCommandDispatcherFactory<TCommand>(
            this QueueClient queueClient,
            IServiceBusMessageSerializer serializer = null,
            Func<TCommand, string> sessionIdProvider = null) where TCommand : ICommand
        {
            return () => new CommandQueueDispatcher<TCommand>(queueClient, serializer ?? new JsonServiceBusMessageSerializer(), sessionIdProvider);
        }
    }
}
