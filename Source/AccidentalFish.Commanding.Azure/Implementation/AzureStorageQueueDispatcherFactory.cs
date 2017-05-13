using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    class AzureStorageQueueDispatcherFactory : IAzureStorageQueueDispatcherFactory
    {
        private readonly IAzureStorageQueueCommandSerializer _serializer;

        public AzureStorageQueueDispatcherFactory(IAzureStorageQueueCommandSerializer serializer)
        {
            _serializer = serializer;
        }

        public ICommandDispatcher Create(CloudQueue queue)
        {
            return new AzureStorageQueueDispatcher(queue, _serializer);
        }
    }
}
