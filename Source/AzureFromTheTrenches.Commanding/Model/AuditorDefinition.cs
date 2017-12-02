using System;

namespace AzureFromTheTrenches.Commanding.Model
{
    internal sealed class AuditorDefinition
    {
        public AuditorDefinition(Type auditorType, bool auditRootCommandOnly)
        {
            AuditorType = auditorType;
            AuditRootCommandOnly = auditRootCommandOnly;
        }

        public Type AuditorType { get; }

        public bool AuditRootCommandOnly { get; }
    }
}
