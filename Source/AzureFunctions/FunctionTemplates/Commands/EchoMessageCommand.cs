using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionTemplates.Commands
{
    public class EchoMessageCommand : ICommand<string>
    {
        public string Message { get; set; }
    }
}
