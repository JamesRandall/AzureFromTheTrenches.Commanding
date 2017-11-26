using System;

namespace AzureFromTheTrenches.Commanding
{
    public class UnableToExecuteHandlerException : Exception
    {
        public UnableToExecuteHandlerException()
        {
        }

        public UnableToExecuteHandlerException(string message) : base(message)
        {
        }

        public UnableToExecuteHandlerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
