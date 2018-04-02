using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureStorage.Implementation;
using AzureFromTheTrenches.Commanding.AzureStorage.Strategies;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFromTheTrenches.Commanding.AzureStorage
{
    // ReSharper disable once InconsistentNaming
    public static class AzureStorageCommandingDependencies
    {
        [Obsolete("Please use AddAzureStorageCommanding instead")]
        public static ICommandingDependencyResolver UseAzureStorageCommanding<TSerializer>(this ICommandingDependencyResolver dependencyResolver) where TSerializer : IAzureStorageQueueSerializer
        {
            Register<TSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        public static ICommandingDependencyResolverAdapter AddAzureStorageCommanding<TSerializer>(this ICommandingDependencyResolverAdapter dependencyResolver) where TSerializer : IAzureStorageQueueSerializer
        {
            Register<TSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        [Obsolete("Please use AddAzureStorageCommanding instead")]
        public static ICommandingDependencyResolver UseAzureStorageCommanding(this ICommandingDependencyResolver dependencyResolver)
        {
            Register<JsonSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        public static ICommandingDependencyResolverAdapter AddAzureStorageCommanding(this ICommandingDependencyResolverAdapter dependencyResolver)
        {
            Register<JsonSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        [Obsolete]
        private static ICommandingDependencyResolver Register<TSerializer>(this ICommandingDependencyResolver dependencyResolver) where TSerializer : IAzureStorageQueueSerializer
        {
            dependencyResolver.TypeMapping<IAzureStorageQueueSerializer, TSerializer>();
            dependencyResolver.TypeMapping<IAzureStorageCommandQueueProcessorFactory, AzureStorageCommandQueueProcessorFactory>();
            dependencyResolver.TypeMapping<IAzureStorageQueueDispatcherFactory, AzureStorageQueueDispatcherFactory>();
            return dependencyResolver;
        }

        private static ICommandingDependencyResolverAdapter Register<TSerializer>(this ICommandingDependencyResolverAdapter dependencyResolver) where TSerializer : IAzureStorageQueueSerializer
        {
            dependencyResolver.TypeMapping<IAzureStorageQueueSerializer, TSerializer>();
            dependencyResolver.TypeMapping<IAzureStorageCommandQueueProcessorFactory, AzureStorageCommandQueueProcessorFactory>();
            dependencyResolver.TypeMapping<IAzureStorageQueueDispatcherFactory, AzureStorageQueueDispatcherFactory>();
            return dependencyResolver;
        }

        /// <summary>
        /// Sets up azure storage command auditing for direct output to tables
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="cloudStorageAccount">The cloud storage account to use for storage</param>
        /// <param name="commandPayloadContainer">(Optional) The blob container that </param>
        /// <param name="storageStrategy"></param>
        /// <param name="options">Auditor options</param>
        /// <returns></returns>
        [Obsolete("Please use AddAzureStorageCommandAuditing instead")]
        public static ICommandingDependencyResolver UseAzureStorageCommandAuditing(this ICommandingDependencyResolver dependencyResolver,
            CloudStorageAccount cloudStorageAccount,            
            CloudBlobContainer commandPayloadContainer = null,
            IStorageStrategy storageStrategy = null,
            AzureStorageAuditorOptions options = null)
        {
            options = options ?? new AzureStorageAuditorOptions();

            if (!options.UseExecutionAuditor && !options.UsePostDispatchAuditor && !options.UsePreDispatchAuditor)
            {
                throw new AzureStorageConfigurationException("At least one auditor type must be configured");
            }
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            if (commandPayloadContainer == null)
            {
                commandPayloadContainer = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference("commandauditpayload");
            }
            if (storageStrategy == null)
            {
                storageStrategy = new SingleTableStrategy();
            }

            ICloudStorageProvider cloudStorageProvider = new CloudStorageProvider(cloudTableClient, commandPayloadContainer);
            dependencyResolver.RegisterInstance(cloudStorageProvider);
            dependencyResolver.RegisterInstance(storageStrategy);
            if (options.UsePreDispatchAuditor)
            {
                dependencyResolver.UsePreDispatchCommandingAuditor<AzureStorageTableCommandAuditor>(options.AuditPreDispatchRootOnly);
            }
            if (options.UsePostDispatchAuditor)
            {
                dependencyResolver.UsePostDispatchCommandingAuditor<AzureStorageTableCommandAuditor>(options.AuditPostDispatchRootOnly);
            }
            if (options.UseExecutionAuditor)
            {
                dependencyResolver.UseExecutionCommandingAuditor<AzureStorageTableCommandAuditor>(options.AuditExecuteDispatchRootOnly);
            }

            return dependencyResolver;
        }

        /// <summary>
        /// Sets up azure storage command auditing for direct output to tables
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="cloudStorageAccount">The cloud storage account to use for storage</param>
        /// <param name="commandPayloadContainer">(Optional) The blob container that </param>
        /// <param name="storageStrategy"></param>
        /// <param name="options">Auditor options</param>
        /// <returns></returns>
        public static ICommandingDependencyResolverAdapter AddAzureStorageCommandAuditing(this ICommandingDependencyResolverAdapter dependencyResolver,
            CloudStorageAccount cloudStorageAccount,
            CloudBlobContainer commandPayloadContainer = null,
            IStorageStrategy storageStrategy = null,
            AzureStorageAuditorOptions options = null)
        {
            options = options ?? new AzureStorageAuditorOptions();

            if (!options.UseExecutionAuditor && !options.UsePostDispatchAuditor && !options.UsePreDispatchAuditor)
            {
                throw new AzureStorageConfigurationException("At least one auditor type must be configured");
            }
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            if (commandPayloadContainer == null)
            {
                commandPayloadContainer = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference("commandauditpayload");
            }
            if (storageStrategy == null)
            {
                storageStrategy = new SingleTableStrategy();
            }

            ICloudStorageProvider cloudStorageProvider = new CloudStorageProvider(cloudTableClient, commandPayloadContainer);
            dependencyResolver.RegisterInstance(cloudStorageProvider);
            dependencyResolver.RegisterInstance(storageStrategy);
            if (options.UsePreDispatchAuditor)
            {
                dependencyResolver.AddPreDispatchCommandingAuditor<AzureStorageTableCommandAuditor>(options.AuditPreDispatchRootOnly);
            }
            if (options.UsePostDispatchAuditor)
            {
                dependencyResolver.AddPostDispatchCommandingAuditor<AzureStorageTableCommandAuditor>(options.AuditPostDispatchRootOnly);
            }
            if (options.UseExecutionAuditor)
            {
                dependencyResolver.AddExecutionCommandingAuditor<AzureStorageTableCommandAuditor>(options.AuditExecuteDispatchRootOnly);
            }

            return dependencyResolver;
        }

        /// <summary>
        /// Sets up azure storage command auditing for output to a queue. This is best suited for scenarios
        /// where there are multiple auditors or storage mechanisms in the audit pipeline as it enables
        /// execution of the command dispatch pipeline to rapidly continue but still with a guarantee
        /// that the command will be audited.
        /// 
        /// Generally when configuring this auditor no other auditors are configured - but you can.
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="queue">The queue to audit via</param>
        /// <param name="blobContainer">The blob container to store the payload to. If this is set then the
        /// payload is stored before the item is queued, if left null then the payload will be serialized
        /// into the queue item. The default setting of null is the more performant and common case, setting
        /// the container here is only useful for very large command payloads that won't fit inside a queue
        /// item. If the payload is stored in the blob container specified here then there will be no way
        /// for downstream auditors to access it from the AuditItem model - it will be null.
        /// </param>
        /// <param name="storageStrategy"></param>
        /// <returns></returns>
        [Obsolete]
        public static ICommandingDependencyResolver UseAzureStorageCommandAuditing(this ICommandingDependencyResolver dependencyResolver,
            CloudQueue queue,
            CloudBlobContainer blobContainer = null,
            IStorageStrategy storageStrategy = null,
            AzureStorageAuditorOptions options = null)
        {
            options = options ?? new AzureStorageAuditorOptions();
            ICloudAuditQueueProvider cloudAuditQueueProvider = new CloudAuditQueueProvider(queue, null);
            ICloudAuditQueueBlobContainerProvider cloudAuditQueueBlobContainerProvider = new CloudAuditQueueBlobContainerProvider(blobContainer);
            dependencyResolver.RegisterInstance(cloudAuditQueueProvider);
            dependencyResolver.RegisterInstance(cloudAuditQueueBlobContainerProvider);
            dependencyResolver.TypeMapping<IAzureStorageQueueSerializer, AzureStorageQueueSerializer>();
            if (options.UsePreDispatchAuditor)
            {
                dependencyResolver.UsePreDispatchCommandingAuditor<AzureStorageQueueCommandAuditor>(options.AuditPreDispatchRootOnly);
            }
            if (options.UsePostDispatchAuditor)
            {
                dependencyResolver.UsePostDispatchCommandingAuditor<AzureStorageQueueCommandAuditor>(options.AuditPostDispatchRootOnly);
            }
            if (options.UseExecutionAuditor)
            {
                dependencyResolver.UseExecutionCommandingAuditor<AzureStorageQueueCommandAuditor>(options.AuditExecuteDispatchRootOnly);
            }
            return dependencyResolver;
        }

        /// <summary>
        /// Sets up azure storage command auditing for output to a queue. This is best suited for scenarios
        /// where there are multiple auditors or storage mechanisms in the audit pipeline as it enables
        /// execution of the command dispatch pipeline to rapidly continue but still with a guarantee
        /// that the command will be audited.
        /// 
        /// Generally when configuring this auditor no other auditors are configured - but you can.
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="queue">The queue to audit via</param>
        /// <param name="blobContainer">The blob container to store the payload to. If this is set then the
        /// payload is stored before the item is queued, if left null then the payload will be serialized
        /// into the queue item. The default setting of null is the more performant and common case, setting
        /// the container here is only useful for very large command payloads that won't fit inside a queue
        /// item. If the payload is stored in the blob container specified here then there will be no way
        /// for downstream auditors to access it from the AuditItem model - it will be null.
        /// </param>
        /// <param name="storageStrategy"></param>
        /// <returns></returns>
        public static ICommandingDependencyResolverAdapter AddAzureStorageCommandAuditing(this ICommandingDependencyResolverAdapter dependencyResolver,
            CloudQueue queue,
            CloudBlobContainer blobContainer = null,
            IStorageStrategy storageStrategy = null,
            AzureStorageAuditorOptions options = null)
        {
            options = options ?? new AzureStorageAuditorOptions();
            ICloudAuditQueueProvider cloudAuditQueueProvider = new CloudAuditQueueProvider(queue, null);
            ICloudAuditQueueBlobContainerProvider cloudAuditQueueBlobContainerProvider = new CloudAuditQueueBlobContainerProvider(blobContainer);
            dependencyResolver.RegisterInstance(cloudAuditQueueProvider);
            dependencyResolver.RegisterInstance(cloudAuditQueueBlobContainerProvider);
            dependencyResolver.TypeMapping<IAzureStorageQueueSerializer, AzureStorageQueueSerializer>();
            if (options.UsePreDispatchAuditor)
            {
                dependencyResolver.AddPreDispatchCommandingAuditor<AzureStorageQueueCommandAuditor>(options.AuditPreDispatchRootOnly);
            }
            if (options.UsePostDispatchAuditor)
            {
                dependencyResolver.AddPostDispatchCommandingAuditor<AzureStorageQueueCommandAuditor>(options.AuditPostDispatchRootOnly);
            }
            if (options.UseExecutionAuditor)
            {
                dependencyResolver.AddExecutionCommandingAuditor<AzureStorageQueueCommandAuditor>(options.AuditExecuteDispatchRootOnly);
            }
            return dependencyResolver;
        }

        /// <summary>
        /// Registers the IAzureStorageCommandQueueProcessorFactory interface through which a audit queue processor task can be
        /// started
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="queue">The queue to dequeue from</param>
        /// <param name="deadLetterQueue">An optional dead letter queue to place items in if errors repeatedly occur in item processing</param>
        /// <returns>The dependency resovler</returns>
        [Obsolete("Please use AddAzureStorageAuditQueueProcessor instead")]
        public static ICommandingDependencyResolver UseAzureStorageAuditQueueProcessor(this ICommandingDependencyResolver dependencyResolver,
            CloudQueue queue, CloudQueue deadLetterQueue = null)
        {
            ICloudAuditQueueProvider cloudAuditQueueProvider = new CloudAuditQueueProvider(queue, deadLetterQueue);
            dependencyResolver.RegisterInstance(cloudAuditQueueProvider);
            dependencyResolver.TypeMapping<IAzureStorageQueueSerializer, AzureStorageQueueSerializer>();
            dependencyResolver.TypeMapping<IAzureStorageAuditQueueProcessorFactory, AzureStorageAuditQueueProcessorFactory>();
            return dependencyResolver;
        }

        /// <summary>
        /// Registers the IAzureStorageCommandQueueProcessorFactory interface through which a audit queue processor task can be
        /// started
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="queue">The queue to dequeue from</param>
        /// <param name="deadLetterQueue">An optional dead letter queue to place items in if errors repeatedly occur in item processing</param>
        /// <returns>The dependency resovler</returns>
        public static ICommandingDependencyResolverAdapter AddAzureStorageAuditQueueProcessor(this ICommandingDependencyResolverAdapter dependencyResolver,
            CloudQueue queue, CloudQueue deadLetterQueue = null)
        {
            ICloudAuditQueueProvider cloudAuditQueueProvider = new CloudAuditQueueProvider(queue, deadLetterQueue);
            dependencyResolver.RegisterInstance(cloudAuditQueueProvider);
            dependencyResolver.TypeMapping<IAzureStorageQueueSerializer, AzureStorageQueueSerializer>();
            dependencyResolver.TypeMapping<IAzureStorageAuditQueueProcessorFactory, AzureStorageAuditQueueProcessorFactory>();
            return dependencyResolver;
        }
    }
}
