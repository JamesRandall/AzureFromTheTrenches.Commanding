using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Azure.ServiceBus;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus
{
    public static class TopicClientDispatcherFactory
    {
        /// <summary>
        /// Create a command dispatcher factory using the connection string and topic name information
        /// </summary>
        /// <param name="connectionString">Sevice bus connection string</param>
        /// <param name="topicName">The name of the topic</param>
        /// <param name="serializer">An optional serializer to use, if unspecified the default JSON serializer will be used</param>
        /// <returns>A command dispatcher factory that will send commands to the topic</returns>
        public static Func<ICommandDispatcher> CreateCommandDispatcherFactory(string connectionString, string topicName, IServiceBusMessageSerializer serializer = null)
        {
            return () =>
            {
                TopicClient client = new TopicClient(connectionString, topicName);
                return client.CreateCommandDispatcher(serializer);
            };
        }

        /// <summary>
        /// Creates a topic command dispatcher factory using the connection string
        /// </summary>
        /// <param name="builder">Connection string builder</param>
        /// <param name="serializer">An optional serializer to use, if unspecified the default JSON serializer will be used</param>
        /// <returns>A command dispatcher factory that will send commands to the topic</returns>
        public static Func<ICommandDispatcher> CreateCommandDispatcherFactory(ServiceBusConnectionStringBuilder builder, IServiceBusMessageSerializer serializer = null)
        {
            return () =>
            {
                TopicClient client = new TopicClient(builder);
                return client.CreateCommandDispatcher(serializer);
            };
        }
    }
}
