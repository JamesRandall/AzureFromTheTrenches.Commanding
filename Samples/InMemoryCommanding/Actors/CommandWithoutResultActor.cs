using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using InMemoryCommanding.Commands;

namespace InMemoryCommanding.Actors
{
    class CommandWithoutResultHandler : ICommandHandler<CommandWithoutResult>
    {
        public Task ExecuteAsync(CommandWithoutResult command)
        {
            Console.WriteLine($"Success - {command.DoSomething}");
            return Task.FromResult(0);
        }
    }
}
