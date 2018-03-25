using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureStorage.Implementation;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureFromTheTrenches.Commanding.AzureStorage
{
    public static class CloudQueueExtensions
    {
        /// <summary>
        /// Creates an Azure storagequeue dispatcher
        /// </summary>
        /// <param name="queue">The queue to dispatch the command to</param>
        /// <param name="serializer">(Optional) Serializer to use, if not specified the default json serializer will be used</param>
        /// <returns></returns>
        public static ICommandDispatcher Create(this CloudQueue queue, IAzureStorageQueueSerializer serializer = null)
        {
            return new AzureStorageQueueDispatcher(queue, serializer ?? new AzureStorageQueueSerializer());
        }
    }
}
