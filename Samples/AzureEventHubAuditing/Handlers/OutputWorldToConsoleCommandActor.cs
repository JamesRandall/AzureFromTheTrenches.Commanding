using System;
using System.Threading.Tasks;
using AzureEventHubAuditing.Commands;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureEventHubAuditing.Handlers
{
    class OutputWorldToConsoleCommandHandler : ICommandHandler<OutputToConsoleCommand, DeferredCommandResult>
    {
        public Task<DeferredCommandResult> ExecuteAsync(OutputToConsoleCommand command, DeferredCommandResult previousResult)
        {
            Console.WriteLine($"{command.Message} World");            
            return Task.FromResult((DeferredCommandResult)null);
        }
    }
}
