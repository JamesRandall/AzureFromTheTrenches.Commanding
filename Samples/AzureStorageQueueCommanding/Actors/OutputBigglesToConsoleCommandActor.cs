using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;
using AccidentalFish.Commanding.Model;
using AzureStorageQueueCommanding.Commands;

namespace AzureStorageQueueCommanding.Actors
{
    class OutputBigglesToConsoleCommandActor : ICommandActor<OutputToConsoleCommand, DeferredCommandResult>
    {
        public Task<DeferredCommandResult> ExecuteAsync(OutputToConsoleCommand command, DeferredCommandResult previousResult)
        {
            Console.WriteLine($"{command.Message} Biggles");
            return Task.FromResult((DeferredCommandResult)null);
        }
    }
}
