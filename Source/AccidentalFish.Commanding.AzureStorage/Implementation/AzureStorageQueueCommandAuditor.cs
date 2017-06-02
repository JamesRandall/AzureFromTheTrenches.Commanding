using System;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.Commanding.AzureStorage.Model;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    internal class AzureStorageQueueCommandAuditor : ICommandAuditor
    {
        private readonly ICloudQueueProvider _cloudQueueProvider;
        private readonly IAzureStorageQueueSerializer _serializer;

        public AzureStorageQueueCommandAuditor(ICloudQueueProvider cloudQueueProvider,
            IAzureStorageQueueSerializer serializer)
        {
            _cloudQueueProvider = cloudQueueProvider;
            _serializer = serializer;
        }

        public async Task AuditWithCommandPayload<TCommand>(TCommand command, Guid commandId, ICommandDispatchContext dispatchContext) where TCommand : class
        {
            string commandType = command.GetType().AssemblyQualifiedName;
            string json = _serializer.Serialize(command);

            CloudBlobContainer blobContainer = _cloudQueueProvider.BlobContainer;
            if (blobContainer != null)
            {
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference($"{commandId}.json");

                await Task.WhenAll(
                    blob.UploadTextAsync(json),
                    AuditWithNoPayload(commandId, commandType, dispatchContext));
            }
            else
            {
                CloudQueue queue = _cloudQueueProvider.Queue;
                DateTime recordedAt = DateTime.UtcNow;

                AuditQueueItem item = new AuditQueueItem
                {
                    AdditionalProperties = dispatchContext.AdditionalProperties.ToDictionary(x => x.Key, x => x.Value.ToString()),
                    CommandType = commandType,
                    CorrelationId = dispatchContext.CorrelationId,
                    Depth = dispatchContext.Depth,
                    CommandId = commandId,
                    CommandPayloadJson = json,
                    RecordedAtUtc = recordedAt
                };
                string queueJson = _serializer.Serialize(item);
                await queue.AddMessageAsync(new CloudQueueMessage(json));
            }
        }

        public async Task AuditWithNoPayload(Guid commandId, string commandType, ICommandDispatchContext dispatchContext)
        {
            CloudQueue queue = _cloudQueueProvider.Queue;
            DateTime recordedAt = DateTime.UtcNow;

            AuditQueueItem item = new AuditQueueItem
            {
                AdditionalProperties = dispatchContext.AdditionalProperties.ToDictionary(x => x.Key, x => x.Value.ToString()),
                CommandType = commandType,
                CorrelationId = dispatchContext.CorrelationId,
                Depth = dispatchContext.Depth,
                CommandId = commandId,
                RecordedAtUtc = recordedAt
            };
            string json = _serializer.Serialize(item);
            await queue.AddMessageAsync(new CloudQueueMessage(json));
        }
    }
}
