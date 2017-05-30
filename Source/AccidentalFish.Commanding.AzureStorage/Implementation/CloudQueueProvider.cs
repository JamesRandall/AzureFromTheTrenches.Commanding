using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    internal class CloudQueueProvider : ICloudQueueProvider
    {
        public CloudQueueProvider(CloudQueue queue, CloudBlobContainer blobContainer)
        {
            Queue = queue;
            BlobContainer = blobContainer;
        }

        public CloudQueue Queue { get; }

        public CloudBlobContainer BlobContainer { get; }
    }
}
