using System;

namespace AccidentalFish.Commanding
{
    public class MissingCommandActorRegistrationException : Exception
    {
        public MissingCommandActorRegistrationException(Type commandType)
        {
            CommandType = commandType;
        }

        public MissingCommandActorRegistrationException(Type commandType, string message) : base(message)
        {
            CommandType = commandType;
        }

        public MissingCommandActorRegistrationException(Type commandType, string message, Exception innerException) : base(message, innerException)
        {
            CommandType = commandType;
        }

        public Type CommandType { get; }
    }
}
