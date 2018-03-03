using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Queue.Implementation
{
    internal class CommandQueueProcessorLogger : ICommandQueueProcessorLogger
    {
        private readonly Action<string, ICommand, Exception> _logWarning;
        private readonly Action<string, ICommand, Exception> _logError;
        private readonly Action<string, ICommand, Exception> _logInfo;

        public CommandQueueProcessorLogger(Action<string, ICommand, Exception> logWarning,
            Action<string, ICommand, Exception> logError,
            Action<string, ICommand, Exception> logInfo)
        {
            _logWarning = logWarning;
            _logError = logError;
            _logInfo = logInfo;
        }

        public void LogInfo(string message, ICommand command, Exception ex)
        {
            _logInfo?.Invoke(message, command, ex);
        }

        public void LogWarning(string message, ICommand command, Exception ex)
        {
            _logWarning?.Invoke(message, command, ex);
        }

        public void LogError(string message, ICommand command, Exception ex)
        {
            _logError?.Invoke(message, command, ex);
        }
    }
}
