using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.AzureStorage.Model;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    class AzureStorageCommandAuditor : ICommandAuditor
    {
        private readonly ICloudStorageProvider _cloudStorageProvider;
        private readonly IStorageStrategy _storageStrategy;

        public AzureStorageCommandAuditor(
            ICloudStorageProvider cloudStorageProvider,
            IStorageStrategy storageStrategy)
        {
            _cloudStorageProvider = cloudStorageProvider;
            _storageStrategy = storageStrategy;
        }

        public async Task Audit<TCommand>(TCommand command, ICommandDispatchContext dispatchContext) where TCommand : class
        {
            DateTime recordedAt = DateTime.UtcNow;
            Guid commandId = Guid.NewGuid();
            string commandType = command.GetType().AssemblyQualifiedName;

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
            string json = JsonConvert.SerializeObject(command);

            Task<CloudBlobContainer> blobContainerTask = _cloudStorageProvider.GetBlobContainer();
            Task<CloudTable> byDateTableTask = _cloudStorageProvider.GetTable(_storageStrategy.GetTableName(byDateDesc));
            Task<CloudTable> byCorrelationIdTableTask = _cloudStorageProvider.GetTable(_storageStrategy.GetTableName(byCorrelationId));

            await Task.WhenAll(blobContainerTask, byDateTableTask, byCorrelationIdTableTask);

            CloudBlockBlob blob = blobContainerTask.Result.GetBlockBlobReference($"{commandId}.json");
            await Task.WhenAll(
                byDateTableTask.Result.ExecuteAsync(TableOperation.Insert(byDateDesc)),
                byCorrelationIdTableTask.Result.ExecuteAsync(TableOperation.Insert(byCorrelationId)),
                blob.UploadTextAsync(json)
            );
        }
    }
}
