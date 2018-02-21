using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Implementation;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers
{
    public class CustomExecuter : ICommandExecuter
    {
        private readonly List<string> _log = new List<string>();

        public Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (command is NoResultCommandWrapper wrappedCommand)
            {
                _log.Add($"Executing command of type {wrappedCommand.Command.GetType().Name} with custom executer");
            }
            else
            {
                _log.Add($"Executing command of type {command.GetType().Name} with custom executer");
            }
            return Task.FromResult(default(TResult));
        }

        public IReadOnlyCollection<string> Log => _log;
    }
}
