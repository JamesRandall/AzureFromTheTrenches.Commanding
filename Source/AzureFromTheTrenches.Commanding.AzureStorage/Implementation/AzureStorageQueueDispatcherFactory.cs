using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureFromTheTrenches.Commanding.AzureStorage.Implementation
{
    class AzureStorageQueueDispatcherFactory : IAzureStorageQueueDispatcherFactory
    {
        private readonly IAzureStorageQueueSerializer _serializer;

        public AzureStorageQueueDispatcherFactory(IAzureStorageQueueSerializer serializer)
        {
            _serializer = serializer;
        }

        public ICommandDispatcher Create(CloudQueue queue)
        {
            return new AzureStorageQueueDispatcher(queue, _serializer);
        }
    }
}
