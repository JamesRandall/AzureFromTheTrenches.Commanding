using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.RabbitMQ.Implementation;
using System;

namespace AzureFromTheTrenches.Commanding.RabbitMQ
{
    public static class QueueClientExtensions
    {
        public static ICommandDispatcher CreateCommandDispatcher(
            this QueueClient queueClient,
            IRabbitMQMessageSerializer serializer = null)
        {
            return new CommandQueueDispatcher(queueClient, serializer ?? new RabbitMQMessageSerializer());
        }

        public static Func<ICommandDispatcher> CreateCommandDispatcherFactory(
            this QueueClient queueClient,
            IRabbitMQMessageSerializer serializer = null)
        {
            return () => new CommandQueueDispatcher(queueClient, serializer ?? new RabbitMQMessageSerializer());
        }
    }
}
