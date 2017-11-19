using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using InMemoryCommanding.Commands;

namespace InMemoryCommanding.Actors
{
    class CommandWithoutResultActor : ICommandActor<CommandWithoutResult>
    {
        public Task ExecuteAsync(CommandWithoutResult command)
        {
            Console.WriteLine($"Success - {command.DoSomething}");
            return Task.FromResult(0);
        }
    }
}
