using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding
{
    /// <summary>
    /// Raised if an error occurred during command dispatch
    /// </summary>
    public class CommandDispatchException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">The command being dispatched</param>
        /// <param name="commandDispatchContext">The dispatch context</param>
        /// <param name="dispatcherType">The type of dispatcher in use</param>
        public CommandDispatchException(object command, ICommandDispatchContext commandDispatchContext, Type dispatcherType)
        {
            Command = command;
            CommandDispatchContext = commandDispatchContext;
            DispatcherType = dispatcherType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">The command being dispatched</param>
        /// <param name="commandDispatchContext">The dispatch context</param>
        /// <param name="dispatcherType">The type of dispatcher in use</param>
        /// <param name="message">Error message</param>
        public CommandDispatchException(object command, ICommandDispatchContext commandDispatchContext, Type dispatcherType, string message) : base(message)
        {
            Command = command;
            CommandDispatchContext = commandDispatchContext;
            DispatcherType = dispatcherType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">The command being dispatched</param>
        /// <param name="commandDispatchContext">The dispatch context</param>
        /// <param name="dispatcherType">The type of dispatcher in use</param>
        /// <param name="message">Error message</param>
        /// <param name="innerException">The inner exceptiont that caused the fault</param>
        public CommandDispatchException(object command, ICommandDispatchContext commandDispatchContext, Type dispatcherType, string message, Exception innerException) : base(message, innerException)
        {
            Command = command;
            CommandDispatchContext = commandDispatchContext;
            DispatcherType = dispatcherType;
        }

        /// <summary>
        /// The command being dispatcher
        /// </summary>
        public object Command { get; }

        /// <summary>
        /// The dispatch context
        /// </summary>
        public ICommandDispatchContext CommandDispatchContext { get; }

        /// <summary>
        /// The type of dispatcher in use
        /// </summary>
        public Type DispatcherType { get; }
    }
}
