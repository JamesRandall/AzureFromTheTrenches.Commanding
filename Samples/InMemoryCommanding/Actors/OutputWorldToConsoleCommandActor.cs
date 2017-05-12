using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using InMemoryCommanding.Commands;
using InMemoryCommanding.Results;

namespace InMemoryCommanding.Actors
{
    class OutputWorldToConsoleCommandActor : ICommandActor<OutputToConsoleCommand, CountResult>
    {
        public Task<CountResult> ExecuteAsync(OutputToConsoleCommand command, CountResult previousResult)
        {
            Console.WriteLine($"{command.Message} World");
            CountResult result = previousResult ?? new CountResult();
            result.Count++;
            return Task.FromResult(result);
        }
    }
}
