using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.AzureEventHub.Model;

namespace AzureFromTheTrenches.Commanding.AzureEventHub.Implementation
{
    internal interface IAuditItemMapper
    {
        EventHubAuditItem Map(AuditItem auditItem);
    }
}
