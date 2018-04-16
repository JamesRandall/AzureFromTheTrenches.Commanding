using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Azure.EventHubs;

namespace AzureFromTheTrenches.Commanding.AzureEventHub
{
    /// <summary>
    /// Shortcuts for creating Event Hub dispatcher factories
    /// </summary>
    public static class AzureEventHubDispatcherFactory
    {
        /// <summary>
        /// Creates an event hub dispatcher factory
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <param name="eventHubName">The event hub name</param>
        /// <param name="serializer">The serializer to use, defaults to the built in JSON serializer</param>
        /// <param name="getPartitionKeyFunc">A function that can be used to establish the partition key given a command</param>
        /// <returns></returns>
        public static Func<ICommandDispatcher> CreateCommandDispatcherFactory(string connectionString, string eventHubName,
            IEventHubSerializer serializer = null, Func<ICommand, string> getPartitionKeyFunc = null)
        {
            EventHubsConnectionStringBuilder connectionStringBuilder =
                new EventHubsConnectionStringBuilder(connectionString) {EntityPath = eventHubName};
            EventHubClient client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            return client.CreateCommandDispatcherFactory(getPartitionKeyFunc);
        }

        /// <summary>
        /// Creates an event hub dispatcher factory
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <param name="serializer">The serializer to use, defaults to the built in JSON serializer</param>
        /// <param name="getPartitionKeyFunc">A function that can be used to establish the partition key given a command</param>
        /// <returns></returns>
        public static Func<ICommandDispatcher> CreateCommandDispatcherFactory(string connectionString,
            IEventHubSerializer serializer = null, Func<ICommand, string> getPartitionKeyFunc = null)
        {
            EventHubClient client = EventHubClient.CreateFromConnectionString(connectionString);
            return client.CreateCommandDispatcherFactory(getPartitionKeyFunc);
        }
    }
}
