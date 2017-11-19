using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.AzureEventHub.Model;
using Newtonsoft.Json;

namespace AzureFromTheTrenches.Commanding.AzureEventHub.Implementation
{
    internal class EventHubSerializer : IEventHubSerializer
    {
        private readonly IAuditItemMapper _auditItemMapper;

        public EventHubSerializer(IAuditItemMapper auditItemMapper)
        {
            _auditItemMapper = auditItemMapper;
        }

        public string Serialize(AuditItem auditItem)
        {
            EventHubAuditItem eventHubAuditItem = _auditItemMapper.Map(auditItem);
            string json = JsonConvert.SerializeObject(eventHubAuditItem);
            return json;
        }
    }
}
