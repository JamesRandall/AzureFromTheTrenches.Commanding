using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Model
{
    internal sealed class AuditorInstance
    {
        public AuditorInstance(ICommandAuditor auditor, bool auditRootCommandOnly)
        {
            Auditor = auditor;
            AuditRootCommandOnly = auditRootCommandOnly;
        }

        public ICommandAuditor Auditor { get; }

        public bool AuditRootCommandOnly { get; }
    }
}
