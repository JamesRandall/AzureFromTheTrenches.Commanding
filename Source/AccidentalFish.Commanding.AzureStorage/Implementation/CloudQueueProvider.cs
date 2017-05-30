using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    internal class CloudQueueProvider : ICloudQueueProvider
    {
        public CloudQueueProvider(CloudQueue queue)
        {
            Queue = queue;
        }

        public CloudQueue Queue { get; }
    }
}
