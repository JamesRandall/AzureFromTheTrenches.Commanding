using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureStorageQueueCommanding.Commands;

namespace AzureStorageQueueCommanding.Actors
{
    class OutputBigglesToConsoleCommandHandler : ICommandHandler<OutputToConsoleCommand, DeferredCommandResult>
    {
        public Task<DeferredCommandResult> ExecuteAsync(OutputToConsoleCommand command, DeferredCommandResult previousResult)
        {
            Console.WriteLine($"{command.Message} Biggles");
            return Task.FromResult((DeferredCommandResult)null);
        }
    }
}
