using AzureFromTheTrenches.Commanding.AzureStorage.Model;

namespace AzureFromTheTrenches.Commanding.AzureStorage
{
    public interface IStorageStrategy
    {
        string GetTableName(CommandAuditByDateDescItem tableEntity);
        string GetTableName(CommandAuditByCorrelationIdItem tableEntity);
        string GetPartitionKey(CommandAuditByDateDescItem tableEntity);
        string GetRowKey(CommandAuditByDateDescItem tableEntity);
        string GetPartitionKey(CommandAuditByCorrelationIdItem tableEntity);
        string GetRowKey(CommandAuditByCorrelationIdItem tableEntity);
    }
}
