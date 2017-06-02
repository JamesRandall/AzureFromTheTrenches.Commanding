using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.AzureStorage.Model;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    class AzureStorageTableCommandAuditor : ICommandAuditor
    {
        private readonly ICloudStorageProvider _cloudStorageProvider;
        private readonly IStorageStrategy _storageStrategy;

        public AzureStorageTableCommandAuditor(
            ICloudStorageProvider cloudStorageProvider,
            IStorageStrategy storageStrategy)
        {
            _cloudStorageProvider = cloudStorageProvider;
            _storageStrategy = storageStrategy;
        }

        public async Task AuditWithCommandPayload<TCommand>(TCommand command, Guid commandId, ICommandDispatchContext dispatchContext) where TCommand : class
        {
            string commandType = command.GetType().AssemblyQualifiedName;
            string json = JsonConvert.SerializeObject(command);
            CloudBlobContainer blobContainer = await _cloudStorageProvider.GetBlobContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference($"{commandId}.json");

            await Task.WhenAll(blob.UploadTextAsync(json),
                AuditWithNoPayload(commandId, commandType, dispatchContext));
        }

        public async Task AuditWithNoPayload(Guid commandId, string commandType, ICommandDispatchContext dispatchContext)
        {
            DateTime recordedAt = DateTime.UtcNow;
            CommandAuditByDateDescItem byDateDesc = new CommandAuditByDateDescItem
            {
                AdditionalProperties = dispatchContext.AdditionalProperties,
                CommandType = commandType,
                CorrelationId = dispatchContext.CorrelationId,
                Depth = dispatchContext.Depth,
                CommandId = commandId,
                RecordedAtUtc = recordedAt                
            };
            byDateDesc.PartitionKey = _storageStrategy.GetPartitionKey(byDateDesc);
            byDateDesc.RowKey = _storageStrategy.GetRowKey(byDateDesc);
            CommandAuditByCorrelationIdItem byCorrelationId = new CommandAuditByCorrelationIdItem
            {
                AdditionalProperties = dispatchContext.AdditionalProperties,
                CommandType = commandType,
                CorrelationId = dispatchContext.CorrelationId,
                Depth = dispatchContext.Depth,
                CommandId = commandId,
                RecordedAtUtc = recordedAt
            };
            byCorrelationId.PartitionKey = _storageStrategy.GetPartitionKey(byCorrelationId);
            byCorrelationId.RowKey = _storageStrategy.GetRowKey(byCorrelationId);
            
            Task<CloudTable> byDateTableTask = _cloudStorageProvider.GetTable(_storageStrategy.GetTableName(byDateDesc));
            Task<CloudTable> byCorrelationIdTableTask = _cloudStorageProvider.GetTable(_storageStrategy.GetTableName(byCorrelationId));

            await Task.WhenAll(byDateTableTask, byCorrelationIdTableTask);

            
            await Task.WhenAll(
                byDateTableTask.Result.ExecuteAsync(TableOperation.Insert(byDateDesc)),
                byCorrelationIdTableTask.Result.ExecuteAsync(TableOperation.Insert(byCorrelationId))
            );
        }
    }
}
