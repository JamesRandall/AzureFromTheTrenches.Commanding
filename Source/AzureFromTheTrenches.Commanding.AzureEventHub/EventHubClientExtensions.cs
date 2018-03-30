using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureEventHub.Implementation;
using EventHubClient = Microsoft.Azure.EventHubs.EventHubClient;

namespace AzureFromTheTrenches.Commanding.AzureEventHub
{
    public static class EventHubClientExtensions
    {
        public static ICommandDispatcher CreateCommandDispatcher(this EventHubClient client,
            Func<ICommand, string> getPartitionKeyFunc = null)
        {
            return new AzureEventHubDispatcher(new Implementation.EventHubClient(client), getPartitionKeyFunc);
        }

        public static Func<ICommandDispatcher> CreateCommandDispatcherFactory(this EventHubClient client,
            Func<ICommand, string> getPartitionKeyFunc = null)
        {
            return () => CreateCommandDispatcher(client, getPartitionKeyFunc);
        }
    }
}
