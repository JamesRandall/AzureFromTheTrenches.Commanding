using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.AzureEventHub.Model;

namespace AzureFromTheTrenches.Commanding.AzureEventHub.Implementation
{
    interface IAuditItemMapper
    {
        EventHubAuditItem Map(AuditItem auditItem);
    }
}
