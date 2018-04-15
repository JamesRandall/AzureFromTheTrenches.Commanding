using System;

namespace AzureFromTheTrenches.Commanding
{
    /// <summary>
    /// Raised when a handler cannot be executed
    /// </summary>
    public class UnableToExecuteHandlerException : Exception
    {
        /// <inheritdoc />
        public UnableToExecuteHandlerException()
        {
        }

        /// <inheritdoc />
        public UnableToExecuteHandlerException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public UnableToExecuteHandlerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
