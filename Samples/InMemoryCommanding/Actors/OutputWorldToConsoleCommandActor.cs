using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using InMemoryCommanding.Commands;
using InMemoryCommanding.Results;

namespace InMemoryCommanding.Actors
{
    class OutputWorldToConsoleCommandHandler : ICommandHandler<OutputToConsoleCommand, CountResult>
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
