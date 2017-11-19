using AzureFromTheTrenches.Commanding.Abstractions;

namespace InMemoryCommanding.Commands
{
    public class CommandWithoutResult : ICommand
    {
        public string DoSomething { get; set; }
    }
}
