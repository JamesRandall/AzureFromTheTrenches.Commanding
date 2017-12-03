using Microsoft.Extensions.Logging;

namespace AzureFromTheTrenches.Commanding.MicrosoftLoggingExtensions.Implementation
{
    internal interface ILogLevelProvider
    {
        LogLevel NormalLogLevel { get; }

        LogLevel ExecutionFailureLogLevel { get; }
    }
}
