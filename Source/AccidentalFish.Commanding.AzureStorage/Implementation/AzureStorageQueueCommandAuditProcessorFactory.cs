using System;
using System.Threading;
using System.Threading.Tasks;
using AccidentalFish.Commanding.AzureStorage.Model;
using AccidentalFish.Commanding.Queue;
using AccidentalFish.Commanding.Queue.Model;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    class AzureStorageQueueCommandAuditProcessorFactory
    {
        private readonly IAsynchronousBackoffPolicyFactory _backoffPolicyFactory;
        private readonly IAzureStorageQueueSerializer _serializer;
        private readonly ICloudQueueProvider _cloudQueueProvider;

        public AzureStorageQueueCommandAuditProcessorFactory(IAsynchronousBackoffPolicyFactory backoffPolicyFactory,
            IAzureStorageQueueSerializer serializer,
            ICloudQueueProvider cloudQueueProvider)
        {
            _backoffPolicyFactory = backoffPolicyFactory;
            _serializer = serializer;
            _cloudQueueProvider = cloudQueueProvider;
        }

        public Task Start(CloudQueue queue, CancellationToken cancellationToken, int maxDequeueCount = 10, Action<string> traceLogger = null)
        {
            AzureStorageQueueBackoffProcessor<AuditQueueItem> queueProcessor = new AzureStorageQueueBackoffProcessor<AuditQueueItem>(
                _backoffPolicyFactory.Create(),
                _serializer,
                queue,
                item => HandleRecievedItemAsync(item, maxDequeueCount),
                traceLogger,
                HandleError);
            return queueProcessor.StartAsync(cancellationToken);
        }

        private Task<bool> HandleError(Exception arg)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> HandleRecievedItemAsync(QueueItem<AuditQueueItem> item, int maxDequeueCount)
        {
            AuditQueueItem auditQueueItem = item.Item;
            try
            {
                // if the command payload was sent on the queue then we need to store it
                if (!string.IsNullOrWhiteSpace(auditQueueItem.CommandPayloadJson))
                {
                    CloudBlobContainer blobContainer = _cloudQueueProvider.BlobContainer;
                    if (blobContainer != null)
                    {
                        CloudBlockBlob blob = blobContainer.GetBlockBlobReference($"{auditQueueItem.CommandId}.json");
                        await blob.UploadTextAsync(auditQueueItem.CommandPayloadJson);
                    }
                }



                return true;
            }
            catch (Exception)
            {
                if (item.DequeueCount > maxDequeueCount)
                {
                    return true;
                }
                return false;
            }            
        }
    }
}
