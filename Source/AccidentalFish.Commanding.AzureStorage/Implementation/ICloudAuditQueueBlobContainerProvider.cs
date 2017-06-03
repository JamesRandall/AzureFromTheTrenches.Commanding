using Microsoft.WindowsAzure.Storage.Blob;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    interface ICloudAuditQueueBlobContainerProvider
    {
        CloudBlobContainer BlobContainer { get; }
    }
}
