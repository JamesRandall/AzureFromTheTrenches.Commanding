using AzureFromTheTrenches.Commanding.Abstractions;

namespace SampleFunctionConfiguration.Commands
{
    public class EchoMessageCommand : ICommand<string>
    {
        public string Message { get; set; }
    }
}
