using Microsoft.WindowsAzure.Storage.Blob;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    class CloudAuditQueueBlobContainerProvider : ICloudAuditQueueBlobContainerProvider
    {
        public CloudAuditQueueBlobContainerProvider(CloudBlobContainer blobContainer)
        {
            BlobContainer = blobContainer;
        }

        public CloudBlobContainer BlobContainer { get; }
    }
}
