using AccidentalFish.Commanding.AzureEventHub.Model;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding.AzureEventHub.Implementation
{
    interface IAuditItemMapper
    {
        EventHubAuditItem Map(AuditItem auditItem);
    }
}
