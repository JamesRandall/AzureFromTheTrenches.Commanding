using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Http.Implementation
{
    class HttpCommandDispatcher : ICommandDispatcher
    {
        public HttpCommandDispatcher(ICommandExecuter httpCommandExecuter)
        {
            AssociatedExecuter = httpCommandExecuter ?? throw new ArgumentNullException(nameof(httpCommandExecuter));
        }

        public Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CommandResult<TResult>(default(TResult), false));
        }

        public Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CommandResult(false));
        }

        public ICommandExecuter AssociatedExecuter { get; }
    }
}
