using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal interface ICommandActorChainExecuter
    {
        Task<CommandChainHandlerResult<TResult>> ExecuteAsync<TResult>(ICommandChainHandler handler, ICommand<TResult> command, TResult previousResult);
    }
}