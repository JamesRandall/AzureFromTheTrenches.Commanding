using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal interface IAuditorRegistration
    {
        void RegisterPreDispatchAuditor<TAuditorImpl>(bool auditRootCommandOnly) where TAuditorImpl : ICommandAuditor;
        void RegisterPostDispatchAuditor<TAuditorImpl>(bool auditRootCommandOnly) where TAuditorImpl : ICommandAuditor;
        void RegisterExecutionAuditor<TAuditorImpl>(bool auditRootCommandOnly) where TAuditorImpl : ICommandAuditor;
    }
}
