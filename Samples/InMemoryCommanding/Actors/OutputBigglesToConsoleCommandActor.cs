using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using InMemoryCommanding.Commands;
using InMemoryCommanding.Results;

namespace InMemoryCommanding.Actors
{
    class OutputBigglesToConsoleCommandActor : ICommandActor<OutputToConsoleCommand, CountResult>
    {
        public Task<CountResult> ExecuteAsync(OutputToConsoleCommand command, CountResult previousResult)
        {
            Console.WriteLine($"{command.Message} Biggles");
            CountResult result = previousResult ?? new CountResult();
            result.Count++;
            return Task.FromResult(result);
        }
    }
}
