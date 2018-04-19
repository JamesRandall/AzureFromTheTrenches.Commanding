using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using SampleFunctionConfiguration.Commands;

namespace SampleFunctionConfiguration.Handlers
{
    internal class EchoMessageCommandHandler : ICommandHandler<EchoMessageCommand, string>
    {
        public Task<string> ExecuteAsync(EchoMessageCommand command, string previousResult)
        {
            return Task.FromResult($"ECHO: {command.Message}");
        }
    }
}
