using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;

namespace AccidentalFish.Commanding.Implementation
{
    interface ICommandActorExecuter
    {
        Task<TResult> ExecuteAsync<TResult>(ICommandActor actor, ICommand<TResult> command, TResult previousResult);
    }
}
