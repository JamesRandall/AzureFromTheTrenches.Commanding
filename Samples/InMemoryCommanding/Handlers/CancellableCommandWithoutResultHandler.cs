using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using InMemoryCommanding.Commands;

namespace InMemoryCommanding.Handlers
{
    class CancellableCommandWithoutResultHandler : ICancellableCommandHandler<CommandWithoutResult>
    {
        public Task ExecuteAsync(CommandWithoutResult command, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Success - {command.DoSomething} - this was cancellable");
            return Task.FromResult(0);
        }
    }
}
