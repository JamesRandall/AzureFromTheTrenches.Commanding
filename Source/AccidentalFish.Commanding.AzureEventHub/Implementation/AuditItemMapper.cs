using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.AzureEventHub.Model;
using Newtonsoft.Json.Linq;

namespace AzureFromTheTrenches.Commanding.AzureEventHub.Implementation
{
    internal class AuditItemMapper : IAuditItemMapper
    {
        public EventHubAuditItem Map(AuditItem auditItem)
        {
            return new EventHubAuditItem
            {
                AdditionalProperties = auditItem.AdditionalProperties,
                Command = new JRaw(auditItem.SerializedCommand),
                CommandId = auditItem.CommandId,
                CommandType = auditItem.CommandType,
                CorrelationId = auditItem.CorrelationId,
                Depth = auditItem.Depth,
                DispatchedUtc = auditItem.DispatchedUtc
            };
        }
    }
}
