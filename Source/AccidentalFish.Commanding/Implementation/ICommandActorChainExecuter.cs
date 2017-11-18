using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;

namespace AccidentalFish.Commanding.Implementation
{
    internal interface ICommandActorChainExecuter
    {
        Task<CommandChainActorResult<TResult>> ExecuteAsync<TResult>(ICommandChainActor actor, ICommand<TResult> command, TResult previousResult);
    }
}