using System;
using AccidentalFish.Commanding.Abstractions;

namespace AccidentalFish.Commanding
{
    public class CommandExecutionException : Exception
    {
        public CommandExecutionException(object command, Type actorType, int actorExecutionIndex, ICommandDispatchContext commandDispatchContext)
        {
            Command = command;
            ActorType = actorType;
            ActorExecutionIndex = actorExecutionIndex;
            CommandDispatchContext = commandDispatchContext;
        }

        public CommandExecutionException(object command, Type actorType, int actorExecutionIndex, ICommandDispatchContext commandDispatchContext, string message) : base(message)
        {
            Command = command;
            ActorType = actorType;
            ActorExecutionIndex = actorExecutionIndex;
            CommandDispatchContext = commandDispatchContext;
        }

        public CommandExecutionException(object command, Type actorType, int actorExecutionIndex, ICommandDispatchContext commandDispatchContext, string message, Exception innerException) : base(message, innerException)
        {
            Command = command;
            ActorType = actorType;
            ActorExecutionIndex = actorExecutionIndex;
            CommandDispatchContext = commandDispatchContext;
        }

        public object Command { get; }

        public Type ActorType { get; }

        public int ActorExecutionIndex { get; }

        public ICommandDispatchContext CommandDispatchContext { get; }
    }

    public class CommandExecutionException<TCommand> : CommandExecutionException where TCommand : class
    {
        public CommandExecutionException(TCommand command, Type actorType, int actorExecutionIndex, ICommandDispatchContext commandDispatchContext) : base(command, actorType, actorExecutionIndex, commandDispatchContext)
        {
        }

        public CommandExecutionException(TCommand command, Type actorType, int actorExecutionIndex, ICommandDispatchContext commandDispatchContext, string message) : base(command, actorType, actorExecutionIndex, commandDispatchContext, message)
        {
        }

        public CommandExecutionException(TCommand command, Type actorType, int actorExecutionIndex, ICommandDispatchContext commandDispatchContext, string message, Exception innerException) : base(command, actorType, actorExecutionIndex, commandDispatchContext, message, innerException)
        {
        }

        public new TCommand Command => (TCommand) base.Command;
    }
}
