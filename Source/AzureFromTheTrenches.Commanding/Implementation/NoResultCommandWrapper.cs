using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class NoResultCommandWrapper : ICommand<NoResult>
    {
        public NoResultCommandWrapper(ICommand command)
        {
            Command = command;
        }

        public ICommand Command { get; }        
    }
}
