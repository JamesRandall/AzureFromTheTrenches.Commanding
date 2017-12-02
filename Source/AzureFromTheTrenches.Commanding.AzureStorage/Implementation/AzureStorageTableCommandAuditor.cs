using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.AzureStorage.Model;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFromTheTrenches.Commanding.AzureStorage.Implementation
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

        private async Task AuditPayload(string payload, string commandId)
        {
            CloudBlobContainer blobContainer = await _cloudStorageProvider.GetBlobContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference($"{commandId}.json");

            await blob.UploadTextAsync(payload);
        }

        public async Task Audit(AuditItem auditItem)
        {
            if (!string.IsNullOrWhiteSpace(auditItem.SerializedCommand))
            {
                await AuditPayload(auditItem.SerializedCommand, auditItem.CommandId);
            }

            CommandAuditByDateDescItem byDateDesc = new CommandAuditByDateDescItem
            {
                AdditionalProperties = auditItem.AdditionalProperties,
                CommandType = auditItem.CommandType,
                CorrelationId = auditItem.CorrelationId,
                Depth = auditItem.Depth,
                CommandId = auditItem.CommandId,
                DispatchedAtUtc = auditItem.DispatchedUtc,
                Type = auditItem.Type
            };
            byDateDesc.PartitionKey = _storageStrategy.GetPartitionKey(byDateDesc);
            byDateDesc.RowKey = _storageStrategy.GetRowKey(byDateDesc);
            CommandAuditByCorrelationIdItem byCorrelationId = new CommandAuditByCorrelationIdItem
            {
                AdditionalProperties = auditItem.AdditionalProperties,
                CommandType = auditItem.CommandType,
                CorrelationId = auditItem.CorrelationId,
                Depth = auditItem.Depth,
                CommandId = auditItem.CommandId,
                DispatchedAtUtc = auditItem.DispatchedUtc,
                Type = auditItem.Type
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
