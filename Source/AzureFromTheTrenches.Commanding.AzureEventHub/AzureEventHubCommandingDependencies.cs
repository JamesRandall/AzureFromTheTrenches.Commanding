using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureEventHub.Implementation;
using Microsoft.Azure.EventHubs;
using EventHubClient = AzureFromTheTrenches.Commanding.AzureEventHub.Implementation.EventHubClient;

namespace AzureFromTheTrenches.Commanding.AzureEventHub
{
    // ReSharper disable once InconsistentNaming
    public static class AzureEventHubCommandingDependencies
    {
        /// <summary>
        /// Registers a command auditor that writes to an event hub
        /// </summary>
        /// <param name="resolver">Dependency resolver</param>
        /// <param name="eventHubClient">The event hub client</param>
        /// <param name="partitionKeyProvider">An optional partition key provider, if unspecified events will be sent unpartitioned</param>
        /// <param name="options">Options for the event hub auditor configuration</param>
        /// <returns>Dependency resolver</returns>
        [Obsolete("Please use AddEventHubCommandAuditing instead")]
        public static ICommandingDependencyResolver UseEventHubCommandAuditing(this ICommandingDependencyResolver resolver,
            Microsoft.Azure.EventHubs.EventHubClient eventHubClient,
            IPartitionKeyProvider partitionKeyProvider = null,
            AzureEventHubAuditorOptions options = null)
        {
            options = options ?? new AzureEventHubAuditorOptions();
            IEventHubClient client = new EventHubClient(eventHubClient);
            if (partitionKeyProvider == null)
            {
                partitionKeyProvider = new NullPartitionKeyProvider();
            }

            resolver.RegisterInstance(client);
            resolver.RegisterInstance(partitionKeyProvider);
            resolver.TypeMapping<IAuditItemMapper, AuditItemMapper>();
            resolver.TypeMapping<IEventHubSerializer, EventHubSerializer>();
            if (options.UsePreDispatchAuditor)
            {
                resolver.UsePreDispatchCommandingAuditor<AzureEventHubCommandAuditor>(options.AuditPreDispatchRootOnly);
            }
            if (options.UsePostDispatchAuditor)
            {
                resolver.UsePostDispatchCommandingAuditor<AzureEventHubCommandAuditor>(options.AuditPostDispatchRootOnly);
            }
            if (options.UseExecutionAuditor)
            {
                resolver.UseExecutionCommandingAuditor<AzureEventHubCommandAuditor>(options.AuditExecuteDispatchRootOnly);
            }
            return resolver;
        }

        /// <summary>
        /// Registers a command auditor that writes to an event hub
        /// </summary>
        /// <param name="resolver">Dependency resolver</param>
        /// <param name="eventHubClient">The event hub client</param>
        /// <param name="partitionKeyProvider">An optional partition key provider, if unspecified events will be sent unpartitioned</param>
        /// <param name="options">Options for the event hub auditor configuration</param>
        /// <returns>Dependency resolver</returns>
        public static ICommandingDependencyResolverAdapter AddEventHubCommandAuditing(this ICommandingDependencyResolverAdapter resolver,
            Microsoft.Azure.EventHubs.EventHubClient eventHubClient,
            IPartitionKeyProvider partitionKeyProvider = null,
            AzureEventHubAuditorOptions options = null)
        {
            options = options ?? new AzureEventHubAuditorOptions();
            IEventHubClient client = new EventHubClient(eventHubClient);
            if (partitionKeyProvider == null)
            {
                partitionKeyProvider = new NullPartitionKeyProvider();
            }

            resolver.RegisterInstance(client);
            resolver.RegisterInstance(partitionKeyProvider);
            resolver.TypeMapping<IAuditItemMapper, AuditItemMapper>();
            resolver.TypeMapping<IEventHubSerializer, EventHubSerializer>();
            if (options.UsePreDispatchAuditor)
            {
                resolver.AddPreDispatchCommandingAuditor<AzureEventHubCommandAuditor>(options.AuditPreDispatchRootOnly);
            }
            if (options.UsePostDispatchAuditor)
            {
                resolver.AddPostDispatchCommandingAuditor<AzureEventHubCommandAuditor>(options.AuditPostDispatchRootOnly);
            }
            if (options.UseExecutionAuditor)
            {
                resolver.AddExecutionCommandingAuditor<AzureEventHubCommandAuditor>(options.AuditExecuteDispatchRootOnly);
            }
            return resolver;
        }

        /// <summary>
        /// Registers a command auditor that writes to an event hub
        /// </summary>
        /// <param name="resolver">Dependency resolver</param>
        /// <param name="connectionString">Connection string to an event hub. This needs to also supply the EntityPath e.g.:
        /// Endpoint=sb://myeventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mysharedaccesskey;EntityPath=myeventhub
        /// </param>
        /// <param name="partitionKeyProvider">An optional partition key provider, if unspecified events will be sent unpartitioned</param>
        /// <param name="options">Options for the event hub auditor configuration</param>
        /// <returns>Dependency resolver</returns>
        [Obsolete("Please use AddEventHubCommandAuditing instead")]
        public static ICommandingDependencyResolver UseEventHubCommandAuditing(this ICommandingDependencyResolver resolver,
            string connectionString,
            IPartitionKeyProvider partitionKeyProvider = null,
            AzureEventHubAuditorOptions options = null)
        {
            Microsoft.Azure.EventHubs.EventHubClient client = Microsoft.Azure.EventHubs.EventHubClient.CreateFromConnectionString(connectionString);
            return UseEventHubCommandAuditing(resolver, client, partitionKeyProvider, options);
        }

        /// <summary>
        /// Registers a command auditor that writes to an event hub
        /// </summary>
        /// <param name="resolver">Dependency resolver</param>
        /// <param name="connectionString">Connection string to an event hub. This needs to also supply the EntityPath e.g.:
        /// Endpoint=sb://myeventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mysharedaccesskey;EntityPath=myeventhub
        /// </param>
        /// <param name="partitionKeyProvider">An optional partition key provider, if unspecified events will be sent unpartitioned</param>
        /// <param name="options">Options for the event hub auditor configuration</param>
        /// <returns>Dependency resolver</returns>        
        public static ICommandingDependencyResolverAdapter AddEventHubCommandAuditing(this ICommandingDependencyResolverAdapter resolver,
            string connectionString,
            IPartitionKeyProvider partitionKeyProvider = null,
            AzureEventHubAuditorOptions options = null)
        {
            Microsoft.Azure.EventHubs.EventHubClient client = Microsoft.Azure.EventHubs.EventHubClient.CreateFromConnectionString(connectionString);
            return AddEventHubCommandAuditing(resolver, client, partitionKeyProvider, options);
        }

        /// <summary>
        /// Registers a command auditor that writes to an event hub
        /// </summary>
        /// <param name="resolver">Dependency resolver</param>
        /// <param name="connectionString">Connection string to an event hub e.g.:
        /// Endpoint=sb://myeventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mysharedaccesskey
        /// </param>
        /// <param name="entityPath">The path to the event hub (usually just the event hub name</param>
        /// <param name="partitionKeyProvider">An optional partition key provider, if unspecified events will be sent unpartitioned</param>
        /// <param name="options">Options for the event hub auditor configuration</param>
        /// <returns>Dependency resolver</returns>
        [Obsolete]
        public static ICommandingDependencyResolver UseEventHubCommandAuditing(this ICommandingDependencyResolver resolver,
            string connectionString,
            string entityPath,
            IPartitionKeyProvider partitionKeyProvider = null,
            AzureEventHubAuditorOptions options = null)
        {
            EventHubsConnectionStringBuilder builder =
                new EventHubsConnectionStringBuilder(connectionString) {EntityPath = entityPath};
            Microsoft.Azure.EventHubs.EventHubClient client = Microsoft.Azure.EventHubs.EventHubClient.CreateFromConnectionString(builder.ToString());
            return UseEventHubCommandAuditing(resolver, client, partitionKeyProvider, options);
        }

        /// <summary>
        /// Registers a command auditor that writes to an event hub
        /// </summary>
        /// <param name="resolver">Dependency resolver</param>
        /// <param name="connectionString">Connection string to an event hub e.g.:
        /// Endpoint=sb://myeventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mysharedaccesskey
        /// </param>
        /// <param name="entityPath">The path to the event hub (usually just the event hub name</param>
        /// <param name="partitionKeyProvider">An optional partition key provider, if unspecified events will be sent unpartitioned</param>
        /// <param name="options">Options for the event hub auditor configuration</param>
        /// <returns>Dependency resolver</returns>
        public static ICommandingDependencyResolverAdapter AddEventHubCommandAuditing(this ICommandingDependencyResolverAdapter resolver,
            string connectionString,
            string entityPath,
            IPartitionKeyProvider partitionKeyProvider = null,
            AzureEventHubAuditorOptions options = null)
        {
            EventHubsConnectionStringBuilder builder =
                new EventHubsConnectionStringBuilder(connectionString) {EntityPath = entityPath};
            Microsoft.Azure.EventHubs.EventHubClient client = Microsoft.Azure.EventHubs.EventHubClient.CreateFromConnectionString(builder.ToString());
            return AddEventHubCommandAuditing(resolver, client, partitionKeyProvider, options);
        }
    }
}
