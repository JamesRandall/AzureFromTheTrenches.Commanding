using System;
using AccidentalFish.Commanding.AzureEventHub.Implementation;
using AccidentalFish.DependencyResolver;
using Microsoft.Azure.EventHubs;
using EventHubClient = AccidentalFish.Commanding.AzureEventHub.Implementation.EventHubClient;

namespace AccidentalFish.Commanding.AzureEventHub
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        /// <summary>
        /// Registers a command auditor that writes to an event hub
        /// </summary>
        /// <param name="resolver">Dependency resolver</param>
        /// <param name="eventHubClient">The event hub client</param>
        /// <param name="partitionKeyProvider">An optional partition key provider, if unspecified events will be sent unpartitioned</param>
        /// <returns>Dependency resolver</returns>
        public static IDependencyResolver UseEventHubCommandAuditing(this IDependencyResolver resolver,
            Microsoft.Azure.EventHubs.EventHubClient eventHubClient,
            IPartitionKeyProvider partitionKeyProvider = null)
        {
            IEventHubClient client = new EventHubClient(eventHubClient);
            if (partitionKeyProvider != null)
            {
                partitionKeyProvider = new NullPartitionKeyProvider();
            }

            resolver.RegisterInstance(client);
            resolver.RegisterInstance(partitionKeyProvider);
            resolver.Register<IAuditItemMapper, AuditItemMapper>();
            resolver.Register<IEventHubSerializer, EventHubSerializer>();

            resolver.RegisterCommandingAuditor<AzureEventHubCommandAuditor>();
            return resolver;
        }

        /// <summary>
        /// Registers a command auditor that writes to an event hub
        /// </summary>
        /// <param name="resolver">Dependency resolver</param>
        /// <param name="connectionString">Connection string to an event hub</param>
        /// <param name="partitionKeyProvider">An optional partition key provider, if unspecified events will be sent unpartitioned</param>
        /// <returns>Dependency resolver</returns>
        public static IDependencyResolver UseEventHubCommandAuditing(this IDependencyResolver resolver,
            string connectionString,
            IPartitionKeyProvider partitionKeyProvider = null)
        {
            Microsoft.Azure.EventHubs.EventHubClient client = Microsoft.Azure.EventHubs.EventHubClient.CreateFromConnectionString(connectionString);
            return UseEventHubCommandAuditing(resolver, client, partitionKeyProvider);
        }
    }
}
