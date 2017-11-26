using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding
{
    public class CommandExecutionException : Exception
    {
        public CommandExecutionException(object command, Type handlerType, int handlerExecutionIndex, ICommandDispatchContext commandDispatchContext)
        {
            Command = command;
            HandlerType = handlerType;
            HandlerExecutionIndex = handlerExecutionIndex;
            CommandDispatchContext = commandDispatchContext;
        }

        public CommandExecutionException(object command, Type handlerType, int handlerExecutionIndex, ICommandDispatchContext commandDispatchContext, string message) : base(message)
        {
            Command = command;
            HandlerType = handlerType;
            HandlerExecutionIndex = handlerExecutionIndex;
            CommandDispatchContext = commandDispatchContext;
        }

        public CommandExecutionException(object command, Type handlerType, int handlerExecutionIndex, ICommandDispatchContext commandDispatchContext, string message, Exception innerException) : base(message, innerException)
        {
            Command = command;
            HandlerType = handlerType;
            HandlerExecutionIndex = handlerExecutionIndex;
            CommandDispatchContext = commandDispatchContext;
        }

        public object Command { get; }

        public Type HandlerType { get; }

        public int HandlerExecutionIndex { get; }

        public ICommandDispatchContext CommandDispatchContext { get; }
    }

    public class CommandExecutionException<TCommand> : CommandExecutionException where TCommand : class
    {
        public CommandExecutionException(TCommand command, Type handlerType, int handlerExecutionIndex, ICommandDispatchContext commandDispatchContext) : base(command, handlerType, handlerExecutionIndex, commandDispatchContext)
        {
        }

        public CommandExecutionException(TCommand command, Type handlerType, int handlerExecutionIndex, ICommandDispatchContext commandDispatchContext, string message) : base(command, handlerType, handlerExecutionIndex, commandDispatchContext, message)
        {
        }

        public CommandExecutionException(TCommand command, Type handlerType, int handlerExecutionIndex, ICommandDispatchContext commandDispatchContext, string message, Exception innerException) : base(command, handlerType, handlerExecutionIndex, commandDispatchContext, message, innerException)
        {
        }

        public new TCommand Command => (TCommand) base.Command;
    }
}
