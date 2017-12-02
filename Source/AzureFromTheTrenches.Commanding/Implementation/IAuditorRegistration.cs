using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal interface IAuditorRegistration
    {
        void RegisterDispatchAuditor<TAuditorImpl>() where TAuditorImpl : ICommandAuditor;
        void RegisterExecutionAuditor<TAuditorImpl>() where TAuditorImpl : ICommandAuditor;
    }
}
