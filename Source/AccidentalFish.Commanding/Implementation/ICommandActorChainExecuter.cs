using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal interface ICommandActorChainExecuter
    {
        Task<CommandChainActorResult<TResult>> ExecuteAsync<TResult>(ICommandChainActor actor, ICommand<TResult> command, TResult previousResult);
    }
}