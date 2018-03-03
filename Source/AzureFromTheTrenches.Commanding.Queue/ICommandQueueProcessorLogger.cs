using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Queue
{
    public interface ICommandQueueProcessorLogger
    {
        void LogInfo(string message, ICommand command = null, Exception ex = null);
        void LogWarning(string message, ICommand command = null, Exception ex = null);
        void LogError(string message, ICommand command = null, Exception ex=null);
    }
}
