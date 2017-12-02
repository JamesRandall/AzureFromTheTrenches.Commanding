using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal interface ICommandHandlerChainExecuter
    {
        Task<CommandChainHandlerResult<TResult>> ExecuteAsync<TResult>(ICommandChainHandler handler, ICommand<TResult> command, TResult previousResult, CancellationToken cancellationToken);
    }
}