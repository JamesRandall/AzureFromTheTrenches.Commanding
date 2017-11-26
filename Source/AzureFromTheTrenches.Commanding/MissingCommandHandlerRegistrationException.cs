using System;

namespace AzureFromTheTrenches.Commanding
{
    public class MissingCommandHandlerRegistrationException : Exception
    {
        public MissingCommandHandlerRegistrationException(Type commandType)
        {
            CommandType = commandType;
        }

        public MissingCommandHandlerRegistrationException(Type commandType, string message) : base(message)
        {
            CommandType = commandType;
        }

        public MissingCommandHandlerRegistrationException(Type commandType, string message, Exception innerException) : base(message, innerException)
        {
            CommandType = commandType;
        }

        public Type CommandType { get; }
    }
}
