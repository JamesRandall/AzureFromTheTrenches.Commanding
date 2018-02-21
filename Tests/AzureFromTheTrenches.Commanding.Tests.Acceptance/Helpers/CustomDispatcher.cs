using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers
{
    public class CustomDispatcher : ICommandDispatcher
    {
        private readonly List<string> _log = new List<string>();

        public Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default(CancellationToken))
        {
            _log.Add($"Command of type {command.GetType().Name} dispatched");
            return Task.FromResult(new CommandResult<TResult>(default(TResult), AssociatedExecuter == null));
        }

        public Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            _log.Add($"Command of type {command.GetType().Name} dispatched");
            return Task.FromResult(new CommandResult(AssociatedExecuter == null));
        }

        public ICommandExecuter AssociatedExecuter { get; set; }

        public IReadOnlyCollection<string> Log => _log;
    }
}
