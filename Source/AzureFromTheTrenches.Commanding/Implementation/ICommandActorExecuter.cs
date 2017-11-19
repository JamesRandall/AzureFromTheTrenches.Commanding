using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    interface ICommandActorExecuter
    {
        Task<TResult> ExecuteAsync<TResult>(ICommandActor actor, ICommand<TResult> command, TResult previousResult);
    }
}
