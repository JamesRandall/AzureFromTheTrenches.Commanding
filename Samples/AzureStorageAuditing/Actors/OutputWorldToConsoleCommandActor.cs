using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.Model;
using AzureStorageAuditing.Commands;

namespace AzureStorageAuditing.Actors
{
    class OutputWorldToConsoleCommandActor : ICommandActor<OutputToConsoleCommand, DeferredCommandResult>
    {
        public Task<DeferredCommandResult> ExecuteAsync(OutputToConsoleCommand command, DeferredCommandResult previousResult)
        {
            Console.WriteLine($"{command.Message} World");            
            return Task.FromResult((DeferredCommandResult)null);
        }
    }
}
