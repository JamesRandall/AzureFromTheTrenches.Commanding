using System;

namespace AccidentalFish.Commanding
{
    public class CommandDispatchException<TCommand> : Exception where TCommand : class
    {
        public CommandDispatchException(TCommand command, ICommandDispatchContext commandDispatchContext, Type dispatcherType)
        {
            Command = command;
            CommandDispatchContext = commandDispatchContext;
            DispatcherType = dispatcherType;
        }
        
        public CommandDispatchException(TCommand command, ICommandDispatchContext commandDispatchContext, Type dispatcherType, string message) : base(message)
        {
            Command = command;
            CommandDispatchContext = commandDispatchContext;
            DispatcherType = dispatcherType;
        }

        public CommandDispatchException(TCommand command, ICommandDispatchContext commandDispatchContext, Type dispatcherType, string message, Exception innerException) : base(message, innerException)
        {
            Command = command;
            CommandDispatchContext = commandDispatchContext;
            DispatcherType = dispatcherType;
        }

        public TCommand Command { get; }

        public ICommandDispatchContext CommandDispatchContext { get; }

        public Type DispatcherType { get; }
    }
}
