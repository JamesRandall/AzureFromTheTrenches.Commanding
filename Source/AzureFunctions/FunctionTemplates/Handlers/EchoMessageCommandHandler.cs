using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionTemplates.Commands;

namespace FunctionTemplates.Handlers
{
    internal class EchoMessageCommandHandler : ICommandHandler<EchoMessageCommand, string>
    {
        public Task<string> ExecuteAsync(EchoMessageCommand command, string previousResult)
        {
            return Task.FromResult($"ECHO: {command.Message}");
        }
    }
}
