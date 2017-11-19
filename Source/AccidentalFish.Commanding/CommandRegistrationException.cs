using System;

namespace AzureFromTheTrenches.Commanding
{
    public class CommandRegistrationException : Exception
    {
        public CommandRegistrationException(string message) : base(message)
        {
        }
    }
}
