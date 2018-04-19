using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionTemplates.Commands
{
    internal class EchoMessageCommand : ICommand<string>
    {
        public string Message { get; set; }
    }
}
