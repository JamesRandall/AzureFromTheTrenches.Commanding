using System;

namespace AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection
{
    class CommandingDependencyInjectorException : Exception
    {
        public CommandingDependencyInjectorException(string message) : base(message)
        {
        }
    }
}
