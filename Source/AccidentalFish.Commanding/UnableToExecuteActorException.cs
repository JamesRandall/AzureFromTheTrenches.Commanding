using System;

namespace AccidentalFish.Commanding
{
    public class UnableToExecuteActorException : Exception
    {
        public UnableToExecuteActorException()
        {
        }

        public UnableToExecuteActorException(string message) : base(message)
        {
        }

        public UnableToExecuteActorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
