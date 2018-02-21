using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace CustomDispatchAndExecuter
{
    public class CustomCommandDispatcher : ICommandDispatcher
    {
        public CustomCommandDispatcher(CustomCommandExecuter commandExecuter)
        {
            AssociatedExecuter = commandExecuter;
        }

        public Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine("CustomCommandDispatcher executing");
            return Task.FromResult(new CommandResult(false)); // setting this to true defers execution - i.e. your executer won't be called
        }

        public ICommandExecuter AssociatedExecuter { get; }
    }
}
