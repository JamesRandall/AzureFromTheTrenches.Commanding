using System;
using System.Threading;
using System.Threading.Tasks;
using AccidentalFish.Commanding.AzureStorage.Implementation;
using AccidentalFish.Commanding.AzureStorage.Strategies;
using AccidentalFish.DependencyResolver;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Commanding.AzureStorage
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseAzureStorageCommanding<TSerializer>(this IDependencyResolver dependencyResolver) where TSerializer : IAzureStorageQueueSerializer
        {
            Register<TSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        public static IDependencyResolver UseAzureStorageCommanding(this IDependencyResolver dependencyResolver)
        {
            Register<JsonSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        private static void Register<TSerializer>(this IDependencyResolver dependencyResolver) where TSerializer : IAzureStorageQueueSerializer
        {
            dependencyResolver.Register<IAzureStorageQueueSerializer, TSerializer>();
            dependencyResolver.Register<IAzureStorageCommandQueueProcessorFactory, AzureStorageCommandQueueProcessorFactory>();
            dependencyResolver.Register<IAzureStorageQueueDispatcherFactory, AzureStorageQueueDispatcherFactory>();
        }

        /// <summary>
        /// Sets up azure storage command auditing for direct output to tables
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="cloudStorageAccount">The cloud storage account to use for storage</param>
        /// <param name="commandPayloadContainer">(Optional) The blob container that </param>
        /// <param name="storageStrategy"></param>
        /// <returns></returns>
        public static IDependencyResolver UseAzureStorageCommandAuditing(this IDependencyResolver dependencyResolver,
            CloudStorageAccount cloudStorageAccount,            
            CloudBlobContainer commandPayloadContainer = null,
            IStorageStrategy storageStrategy = null)
        {
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
            dependencyResolver.RegisterCommandingAuditor<AzureStorageTableCommandAuditor>();
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
        /// item.
        /// </param>
        /// <param name="storageStrategy"></param>
        /// <returns></returns>
        public static IDependencyResolver UseAzureStorageCommandAuditing(this IDependencyResolver dependencyResolver,
            CloudQueue queue,
            CloudBlobContainer blobContainer = null,
            IStorageStrategy storageStrategy = null)
        {
            ICloudQueueProvider cloudQueueProvider = new CloudQueueProvider(queue, blobContainer);
            dependencyResolver.RegisterInstance(cloudQueueProvider);
            dependencyResolver.RegisterCommandingAuditor<AzureStorageQueueCommandAuditor>();
            return dependencyResolver;
        }

        
        public static IDependencyResolver UseAzureStorageAuditQueueProcessor(this IDependencyResolver dependencyResolver)
        {

            return dependencyResolver;
        }
    }
}
