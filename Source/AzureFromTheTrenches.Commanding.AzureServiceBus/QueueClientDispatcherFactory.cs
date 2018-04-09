using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Azure.ServiceBus;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus
{
    public static class QueueClientDispatcherFactory
    {
        /// <summary>
        /// Create a command dispatcher factory using the connection string and queue name information
        /// </summary>
        /// <param name="connectionString">Sevice bus connection string</param>
        /// <param name="queueName">The name of the queue</param>
        /// <param name="serializer">An optional serializer to use, if unspecified the default JSON serializer will be used</param>
        /// <returns>A command dispatcher factory that will send commands to the queue</returns>
        public static Func<ICommandDispatcher> CreateCommandDispatcherFactory(string connectionString, string queueName, IServiceBusMessageSerializer serializer=null)
        {
            return () =>
            {
                QueueClient client = new QueueClient(connectionString, queueName);
                return client.CreateCommandDispatcher(serializer);
            };
        }

        /// <summary>
        /// Creates a queue command dispatcher factory using the connection string
        /// </summary>
        /// <param name="builder">Connection string builder</param>
        /// <param name="serializer">An optional serializer to use, if unspecified the default JSON serializer will be used</param>
        /// <returns>A command dispatcher factory that will send commands to the queue</returns>
        public static Func<ICommandDispatcher> CreateCommandDispatcherFactory(ServiceBusConnectionStringBuilder builder, IServiceBusMessageSerializer serializer = null)
        {
            return () =>
            {
                QueueClient client = new QueueClient(builder);
                return client.CreateCommandDispatcher(serializer);
            };
        }
    }
}
