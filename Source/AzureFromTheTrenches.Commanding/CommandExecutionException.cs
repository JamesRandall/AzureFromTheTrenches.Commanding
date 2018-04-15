using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding
{
    /// <summary>
    /// Raised if an error occurs during command execution
    /// </summary>
    public class CommandExecutionException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="handlerType">The handler in use when the execution failed</param>
        /// <param name="handlerExecutionIndex">The index of the handler in the pipeline</param>
        /// <param name="commandDispatchContext">The dispatch context</param>
        public CommandExecutionException(object command, Type handlerType, int handlerExecutionIndex, ICommandDispatchContext commandDispatchContext)
        {
            Command = command;
            HandlerType = handlerType;
            HandlerExecutionIndex = handlerExecutionIndex;
            CommandDispatchContext = commandDispatchContext;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="handlerType">The handler in use when the execution failed</param>
        /// <param name="handlerExecutionIndex">The index of the handler in the pipeline</param>
        /// <param name="commandDispatchContext">The dispatch context</param>
        /// <param name="message">Error message</param>
        public CommandExecutionException(object command, Type handlerType, int handlerExecutionIndex, ICommandDispatchContext commandDispatchContext, string message) : base(message)
        {
            Command = command;
            HandlerType = handlerType;
            HandlerExecutionIndex = handlerExecutionIndex;
            CommandDispatchContext = commandDispatchContext;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="handlerType">The handler in use when the execution failed</param>
        /// <param name="handlerExecutionIndex">The index of the handler in the pipeline</param>
        /// <param name="commandDispatchContext">The dispatch context</param>
        /// <param name="message">Error message</param>
        /// <param name="innerException">The exception that caused the fault</param>
        public CommandExecutionException(object command, Type handlerType, int handlerExecutionIndex, ICommandDispatchContext commandDispatchContext, string message, Exception innerException) : base(message, innerException)
        {
            Command = command;
            HandlerType = handlerType;
            HandlerExecutionIndex = handlerExecutionIndex;
            CommandDispatchContext = commandDispatchContext;
        }

        /// <summary>
        /// The command
        /// </summary>
        public object Command { get; }

        /// <summary>
        /// The type of the handler
        /// </summary>
        public Type HandlerType { get; }

        /// <summary>
        /// The index of the handler in the pipeline
        /// </summary>
        public int HandlerExecutionIndex { get; }

        /// <summary>
        /// The command dispatch context
        /// </summary>
        public ICommandDispatchContext CommandDispatchContext { get; }
    }


    /// <inheritdoc />
    public class CommandExecutionException<TCommand> : CommandExecutionException where TCommand : class
    {
        /// <inheritdoc />
        public CommandExecutionException(TCommand command, Type handlerType, int handlerExecutionIndex, ICommandDispatchContext commandDispatchContext) : base(command, handlerType, handlerExecutionIndex, commandDispatchContext)
        {
        }

        /// <inheritdoc />
        public CommandExecutionException(TCommand command, Type handlerType, int handlerExecutionIndex, ICommandDispatchContext commandDispatchContext, string message) : base(command, handlerType, handlerExecutionIndex, commandDispatchContext, message)
        {
        }

        /// <inheritdoc />
        public CommandExecutionException(TCommand command, Type handlerType, int handlerExecutionIndex, ICommandDispatchContext commandDispatchContext, string message, Exception innerException) : base(command, handlerType, handlerExecutionIndex, commandDispatchContext, message, innerException)
        {
        }

        /// <summary>
        /// The command
        /// </summary>
        public new TCommand Command => (TCommand) base.Command;
    }
}
