using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class DirectCommandExecuter : IDirectCommandExecuter
    {
        private readonly ICommandScopeManager _commandScopeManager;
        private readonly ICommandExecuter _commandExecuter;

        public DirectCommandExecuter(ICommandScopeManager commandScopeManager, ICommandExecuter commandExecuter)
        {
            _commandScopeManager = commandScopeManager;
            _commandExecuter = commandExecuter;
        }

        public async Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default(CancellationToken))
        {
            _commandScopeManager.Enter();
            try
            {
                return await _commandExecuter.ExecuteAsync(command, cancellationToken);
            }
            finally
            {
                _commandScopeManager.Exit();
            }
        }
    }
}
