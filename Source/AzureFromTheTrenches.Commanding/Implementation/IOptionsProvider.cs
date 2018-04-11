using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal interface IOptionsProvider
    {
        IOptions Options { get; }
    }
}
