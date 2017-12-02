using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal interface IAuditorRegistration
    {
        void RegisterDispatchAuditor<TAuditorImpl>(bool auditRootCommandOnly) where TAuditorImpl : ICommandAuditor;
        void RegisterExecutionAuditor<TAuditorImpl>(bool auditRootCommandOnly) where TAuditorImpl : ICommandAuditor;
    }
}
