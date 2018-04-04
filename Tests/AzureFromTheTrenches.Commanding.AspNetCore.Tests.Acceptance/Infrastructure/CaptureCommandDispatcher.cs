using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Infrastructure
{
    class CaptureCommandDispatcher : ICommandDispatcher
    {
        private readonly List<ICommand> _commandLog;

        public CaptureCommandDispatcher()
        {
            _commandLog = new List<ICommand>();
        }

        public Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default(CancellationToken))
        {
            _commandLog.Add(command);
            return Task.FromResult<CommandResult<TResult>>(new CommandResult<TResult>(default(TResult), true));
        }

        public Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            _commandLog.Add(command);
            return Task.FromResult<CommandResult>(new CommandResult(true));
        }

        public IReadOnlyCollection<ICommand> CommandLog => _commandLog;

        public ICommandExecuter AssociatedExecuter { get; }
    }
}
