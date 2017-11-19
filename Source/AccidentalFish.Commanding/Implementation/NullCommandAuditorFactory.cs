using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    class NullCommandAuditorFactory : ICommandAuditorFactory
    {
        public ICommandAuditor Create<TCommand>() where TCommand : class
        {
            return null;
        }
    }
}
