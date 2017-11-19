using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.AzureEventHub.Implementation
{
    internal class NullPartitionKeyProvider : IPartitionKeyProvider
    {
        public string GetPartitionKey(AuditItem auditItem)
        {
            return null;
        }
    }
}
