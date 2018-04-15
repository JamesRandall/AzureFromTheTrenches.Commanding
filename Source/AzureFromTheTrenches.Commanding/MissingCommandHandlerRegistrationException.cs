using System;

namespace AzureFromTheTrenches.Commanding
{
    /// <summary>
    /// Raised if a command handler cannot be found for a command
    /// </summary>
    public class MissingCommandHandlerRegistrationException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandType">The type of the command</param>
        public MissingCommandHandlerRegistrationException(Type commandType)
        {
            CommandType = commandType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandType">The type of the command</param>
        /// <param name="message">Error message</param>
        public MissingCommandHandlerRegistrationException(Type commandType, string message) : base(message)
        {
            CommandType = commandType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandType">The type of the command</param>
        /// <param name="message">Error message</param>
        /// <param name="innerException">The inner exception that caused the issue</param>
        public MissingCommandHandlerRegistrationException(Type commandType, string message, Exception innerException) : base(message, innerException)
        {
            CommandType = commandType;
        }

        /// <summary>
        /// The type of the command
        /// </summary>
        public Type CommandType { get; }
    }
}
