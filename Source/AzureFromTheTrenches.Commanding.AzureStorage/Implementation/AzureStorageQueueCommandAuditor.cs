using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureFromTheTrenches.Commanding.AzureStorage.Implementation
{
    internal class AzureStorageQueueCommandAuditor : ICommandAuditor
    {
        private readonly ICloudAuditQueueProvider _cloudAuditQueueProvider;
        private readonly ICloudAuditQueueBlobContainerProvider _blobContainerProvider;
        private readonly IAzureStorageQueueSerializer _serializer;

        public AzureStorageQueueCommandAuditor(ICloudAuditQueueProvider cloudAuditQueueProvider,
            ICloudAuditQueueBlobContainerProvider blobContainerProvider,
            IAzureStorageQueueSerializer serializer)
        {
            _cloudAuditQueueProvider = cloudAuditQueueProvider;
            _blobContainerProvider = blobContainerProvider;
            _serializer = serializer;
        }

        public async Task Audit(AuditItem auditItem)
        {
            CloudBlobContainer blobContainer = _blobContainerProvider.BlobContainer;
            if (blobContainer != null && !string.IsNullOrWhiteSpace(auditItem.SerializedCommand))
            {
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference($"{auditItem.CommandId}.json");
                await blob.UploadTextAsync(auditItem.SerializedCommand);
                auditItem.SerializedCommand = null;
            }
            CloudQueue queue = _cloudAuditQueueProvider.Queue;
            string queueItemJson = _serializer.Serialize(auditItem);
            await queue.AddMessageAsync(new CloudQueueMessage(queueItemJson));
        }        
    }
}
