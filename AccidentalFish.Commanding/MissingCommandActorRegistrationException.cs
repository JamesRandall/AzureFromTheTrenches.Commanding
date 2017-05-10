using System;

namespace AccidentalFish.Commanding
{
    public class MissingCommandActorRegistrationException : Exception
    {
        public MissingCommandActorRegistrationException(Type type)
        {
            Type = type;
        }

        public MissingCommandActorRegistrationException(Type type, string message) : base(message)
        {
            Type = type;
        }

        public MissingCommandActorRegistrationException(Type type, string message, Exception innerException) : base(message, innerException)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}
