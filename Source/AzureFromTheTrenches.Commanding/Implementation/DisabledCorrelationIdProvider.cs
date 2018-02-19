using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    class DisabledCorrelationIdProvider : ICommandCorrelationIdProvider
    {
        public string Create()
        {
            return string.Empty;
        }
    }
}
