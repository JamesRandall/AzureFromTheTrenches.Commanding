using System;

namespace AccidentalFish.Commanding
{
    public class CommandDispatchException<TCommand> : Exception where TCommand : class
    {
        public CommandDispatchException(TCommand command, ICommandContext commandContext, Type dispatcherType)
        {
            Command = command;
            CommandContext = commandContext;
            DispatcherType = dispatcherType;
        }
        
        public CommandDispatchException(TCommand command, ICommandContext commandContext, Type dispatcherType, string message) : base(message)
        {
            Command = command;
            CommandContext = commandContext;
            DispatcherType = dispatcherType;
        }

        public CommandDispatchException(TCommand command, ICommandContext commandContext, Type dispatcherType, string message, Exception innerException) : base(message, innerException)
        {
            Command = command;
            CommandContext = commandContext;
            DispatcherType = dispatcherType;
        }

        public TCommand Command { get; }

        public ICommandContext CommandContext { get; }

        public Type DispatcherType { get; }
    }
}
