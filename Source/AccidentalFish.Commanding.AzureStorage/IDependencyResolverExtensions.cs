using System;
using AccidentalFish.Commanding.AzureStorage.Implementation;
using AccidentalFish.DependencyResolver;
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

        public static IDependencyResolver UseAzureStorageCommandAuditing(this IDependencyResolver dependencyResolver,
            CloudTable auditByCorrelationIdTable,
            CloudTable auditByDateDescTable,
            CloudBlobContainer commandPayloadContainer)
        {
            if (auditByCorrelationIdTable == null) throw new ArgumentNullException(nameof(auditByCorrelationIdTable));
            if (auditByDateDescTable == null) throw new ArgumentNullException(nameof(auditByDateDescTable));
            if (commandPayloadContainer == null) throw new ArgumentNullException(nameof(commandPayloadContainer));

            IAzureStorageCommandAuditorConfiguration configuration = new AzureStorageCommandAuditorConfiguration(auditByCorrelationIdTable, auditByDateDescTable, commandPayloadContainer);
            dependencyResolver.RegisterInstance(configuration);
            dependencyResolver.Register<ICommandAuditorFactory, AzureStorageCommandAuditorFactory>();
            return dependencyResolver;
        }
    }
}
