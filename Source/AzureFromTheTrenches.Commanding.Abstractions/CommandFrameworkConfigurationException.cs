using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public class CommandFrameworkConfigurationException : Exception
    {
        public CommandFrameworkConfigurationException(string message) : base(message)
        {
        }
    }
}
