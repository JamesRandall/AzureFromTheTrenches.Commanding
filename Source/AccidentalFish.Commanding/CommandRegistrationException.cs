using System;

namespace AccidentalFish.Commanding
{
    public class CommandRegistrationException : Exception
    {
        public CommandRegistrationException(string message) : base(message)
        {
        }
    }
}
