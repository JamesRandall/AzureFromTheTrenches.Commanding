using AzureFromTheTrenches.Commanding.Abstractions;

namespace SampleFunctionConfiguration.Commands
{
    internal class EchoMessageCommand : ICommand<string>
    {
        public string Message { get; set; }
    }
}
