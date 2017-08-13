using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding.AzureEventHub.Implementation
{
    internal class NullPartitionKeyProvider : IPartitionKeyProvider
    {
        public string GetPartitionKey(AuditItem auditItem)
        {
            return null;
        }
    }
}
