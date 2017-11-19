using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding
{
    public class CommandDispatchException : Exception
    {
        public CommandDispatchException(object command, ICommandDispatchContext commandDispatchContext, Type dispatcherType)
        {
            Command = command;
            CommandDispatchContext = commandDispatchContext;
            DispatcherType = dispatcherType;
        }
        
        public CommandDispatchException(object command, ICommandDispatchContext commandDispatchContext, Type dispatcherType, string message) : base(message)
        {
            Command = command;
            CommandDispatchContext = commandDispatchContext;
            DispatcherType = dispatcherType;
        }

        public CommandDispatchException(object command, ICommandDispatchContext commandDispatchContext, Type dispatcherType, string message, Exception innerException) : base(message, innerException)
        {
            Command = command;
            CommandDispatchContext = commandDispatchContext;
            DispatcherType = dispatcherType;
        }

        public object Command { get; }

        public ICommandDispatchContext CommandDispatchContext { get; }

        public Type DispatcherType { get; }
    }
}
