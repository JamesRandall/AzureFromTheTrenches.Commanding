using System;

namespace AccidentalFish.Commanding
{
    public class CommandExecutionException : Exception
    {
        public CommandExecutionException(object command, Type actorType, int actorExecutionIndex, ICommandContext commandContext)
        {
            Command = command;
            ActorType = actorType;
            ActorExecutionIndex = actorExecutionIndex;
            CommandContext = commandContext;
        }

        public CommandExecutionException(object command, Type actorType, int actorExecutionIndex, ICommandContext commandContext, string message) : base(message)
        {
            Command = command;
            ActorType = actorType;
            ActorExecutionIndex = actorExecutionIndex;
            CommandContext = commandContext;
        }

        public CommandExecutionException(object command, Type actorType, int actorExecutionIndex, ICommandContext commandContext, string message, Exception innerException) : base(message, innerException)
        {
            Command = command;
            ActorType = actorType;
            ActorExecutionIndex = actorExecutionIndex;
            CommandContext = commandContext;
        }

        public object Command { get; }

        public Type ActorType { get; }

        public int ActorExecutionIndex { get; }

        public ICommandContext CommandContext { get; }
    }

    public class CommandExecutionException<TCommand> : CommandExecutionException where TCommand : class
    {
        public CommandExecutionException(TCommand command, Type actorType, int actorExecutionIndex, ICommandContext commandContext) : base(command, actorType, actorExecutionIndex, commandContext)
        {
        }

        public CommandExecutionException(TCommand command, Type actorType, int actorExecutionIndex, ICommandContext commandContext, string message) : base(command, actorType, actorExecutionIndex, commandContext, message)
        {
        }

        public CommandExecutionException(TCommand command, Type actorType, int actorExecutionIndex, ICommandContext commandContext, string message, Exception innerException) : base(command, actorType, actorExecutionIndex, commandContext, message, innerException)
        {
        }

        public new TCommand Command => (TCommand) base.Command;
    }
}
