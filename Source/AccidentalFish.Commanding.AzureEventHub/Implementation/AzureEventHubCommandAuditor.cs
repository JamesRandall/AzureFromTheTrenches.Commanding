using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding.AzureEventHub.Implementation
{
    class AzureEventHubCommandAuditor : ICommandAuditor
    {
        private readonly IEventHubClient _client;
        private readonly IEventHubSerializer _serializer;
        private readonly IPartitionKeyProvider _partitionKeyProvider;

        public AzureEventHubCommandAuditor(IEventHubClient client, IEventHubSerializer serializer, IPartitionKeyProvider partitionKeyProvider)
        {
            _client = client;
            _serializer = serializer;
            _partitionKeyProvider = partitionKeyProvider;
        }

        public async Task Audit(AuditItem auditItem)
        {
            string messageText = _serializer.Serialize(auditItem);
            string partitionKey = _partitionKeyProvider.GetPartitionKey(auditItem);
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                await _client.SendAsync(messageText);
            }
            else
            {
                await _client.SendAsync(messageText, partitionKey);
            }
        }
    }
}
