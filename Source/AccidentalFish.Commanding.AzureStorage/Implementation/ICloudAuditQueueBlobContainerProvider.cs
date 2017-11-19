using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureFromTheTrenches.Commanding.AzureStorage.Implementation
{
    interface ICloudAuditQueueBlobContainerProvider
    {
        CloudBlobContainer BlobContainer { get; }
    }
}
