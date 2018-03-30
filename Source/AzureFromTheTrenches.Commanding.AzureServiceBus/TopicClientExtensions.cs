using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureServiceBus.Implementation;
using Microsoft.Azure.ServiceBus;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus
{
    public static class TopicClientExtensions
    {
        public static ICommandDispatcher CreateCommandDispatcher(
            this TopicClient topicClient,
            IServiceBusMessageSerializer serializer = null)
        {
            return new CommandTopicDispatcher(topicClient, serializer ?? new JsonServiceBusMessageSerializer());
        }

        public static Func<ICommandDispatcher> CreateCommandDispatcherFactory(
            this TopicClient topicClient,
            IServiceBusMessageSerializer serializer = null)
        {
            return () => CreateCommandDispatcher(topicClient, serializer);
        }
    }
}
