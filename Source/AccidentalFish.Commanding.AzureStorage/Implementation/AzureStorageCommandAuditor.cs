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
        private readonly CloudTable _auditByDateDescTable;
        private readonly CloudTable _auditByCorrelationIdTable;
        private readonly CloudBlobContainer _blobContainer;

        public AzureStorageCommandAuditor(
            CloudTable auditByDateDescTable,
            CloudTable auditByCorrelationIdTable,
            CloudBlobContainer blobContainer)
        {
            _auditByDateDescTable = auditByDateDescTable;
            _auditByCorrelationIdTable = auditByCorrelationIdTable;
            _blobContainer = blobContainer;
        }

        public async Task Audit<TCommand>(TCommand command, ICommandContext context) where TCommand : class
        {
            DateTime recordedAt = DateTime.UtcNow;
            Guid commandId = Guid.NewGuid();
            string commandType = command.GetType().AssemblyQualifiedName;

            CommandAuditByDateDescItem byDateDesc = new CommandAuditByDateDescItem
            {
                CommandType = commandType,
                CorrelationId = context.CorrelationId,
                Depth = context.Depth,
                PartitionKey = CommandAuditByDateDescItem.GetPartitionKey(recordedAt),
                RecordedAtUtc = recordedAt,
                RowKey = CommandAuditByDateDescItem.GetRowKey(commandId)
            };
            CommandAuditByCorrelationIdItem byCorrelationId = new CommandAuditByCorrelationIdItem
            {
                CommandId = commandId,
                CommandType = commandType,
                Depth = context.Depth,
                PartitionKey = CommandAuditByCorrelationIdItem.GetPartitionKey(context.CorrelationId),
                RecordedAtUtc = recordedAt,
                RowKey = CommandAuditByCorrelationIdItem.GetRowKey(recordedAt, commandId)
            };
            string json = JsonConvert.SerializeObject(command);
            CloudBlockBlob blob = _blobContainer.GetBlockBlobReference($"{commandId}.json");
            /*await Task.WhenAll(
                _auditByDateDescTable.ExecuteAsync(TableOperation.Insert(byDateDesc)),
                _auditByCorrelationIdTable.ExecuteAsync(TableOperation.Insert(byCorrelationId)),
                blob.UploadTextAsync(json)
            );*/

            await _auditByDateDescTable.ExecuteAsync(TableOperation.Insert(byDateDesc));
            await _auditByCorrelationIdTable.ExecuteAsync(TableOperation.Insert(byCorrelationId));
            await blob.UploadTextAsync(json);
        }
    }
}
