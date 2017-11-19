using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.AzureEventHub
{
    public interface IPartitionKeyProvider
    {
        string GetPartitionKey(AuditItem auditItem);
    }
}
