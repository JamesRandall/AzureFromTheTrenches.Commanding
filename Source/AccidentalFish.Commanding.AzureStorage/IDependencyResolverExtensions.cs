using System;
using AccidentalFish.Commanding.AzureStorage.Implementation;
using AccidentalFish.Commanding.AzureStorage.Strategies;
using AccidentalFish.DependencyResolver;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Commanding.AzureStorage
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseAzureStorageCommanding<TSerializer>(this IDependencyResolver dependencyResolver) where TSerializer : IAzureStorageQueueCommandSerializer
        {
            Register<TSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        public static IDependencyResolver UseAzureStorageCommanding(this IDependencyResolver dependencyResolver)
        {
            Register<JsonCommandSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        private static void Register<TSerializer>(this IDependencyResolver dependencyResolver) where TSerializer : IAzureStorageQueueCommandSerializer
        {
            dependencyResolver.Register<IAzureStorageQueueCommandSerializer, TSerializer>();
            dependencyResolver.Register<IAzureStorageCommandQueueProcessorFactory, AzureStorageCommandQueueProcessorFactory>();
            dependencyResolver.Register<IAzureStorageQueueDispatcherFactory, AzureStorageQueueDispatcherFactory>();
        }

        /// <summary>
        /// Sets up azure storage command auditing
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
                commandPayloadContainer = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference("commandauditbydate");
            }
            if (storageStrategy == null)
            {
                storageStrategy = new SingleTableStrategy();
            }

            ICloudStorageProvider cloudStorageProvider = new CloudStorageProvider(cloudTableClient, commandPayloadContainer);
            dependencyResolver.RegisterInstance(cloudStorageProvider);
            dependencyResolver.RegisterInstance(storageStrategy);
            dependencyResolver.RegisterCommandingAuditor<AzureStorageCommandAuditor>();
            return dependencyResolver;
        }
    }
}
