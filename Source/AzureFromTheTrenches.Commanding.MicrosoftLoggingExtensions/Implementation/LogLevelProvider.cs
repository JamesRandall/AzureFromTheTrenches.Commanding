using Microsoft.Extensions.Logging;

namespace AzureFromTheTrenches.Commanding.MicrosoftLoggingExtensions.Implementation
{
    internal class LogLevelProvider : ILogLevelProvider
    {
        public LogLevelProvider(LogLevel normalLogLevel, LogLevel executionFailureLogLevel)
        {
            NormalLogLevel = normalLogLevel;
            ExecutionFailureLogLevel = executionFailureLogLevel;
        }

        public LogLevel NormalLogLevel { get; }
        public LogLevel ExecutionFailureLogLevel { get; }
    }
}
