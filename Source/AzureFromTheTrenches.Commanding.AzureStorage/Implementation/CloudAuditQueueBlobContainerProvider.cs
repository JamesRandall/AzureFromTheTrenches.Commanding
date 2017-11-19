using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureFromTheTrenches.Commanding.AzureStorage.Implementation
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
