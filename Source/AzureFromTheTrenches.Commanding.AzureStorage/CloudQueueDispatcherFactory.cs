using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureStorage.Implementation;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureFromTheTrenches.Commanding.AzureStorage
{
    public static class CloudQueueDispatcherFactory
    {
        public static Func<ICommandDispatcher> Create(string storageAccountConnectionString, string queueName, IAzureStorageQueueSerializer serializer = null)
        {
            return () =>
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
                CloudQueue queue = storageAccount.CreateCloudQueueClient().GetQueueReference(queueName);
                return new AzureStorageQueueDispatcher(queue, serializer ?? new AzureStorageQueueSerializer());
            };
        }
    }
}
