using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions.Model;

namespace AccidentalFish.Commanding.Abstractions
{
    public interface ICommandActorBase
    {

    }

    public interface ICommandActor : ICommandActorBase
    {
        
    }

    public interface ICommandChainActor : ICommandActorBase
    {
        
    }

    public interface ICommandActor<in TCommand> : ICommandActor where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command);
    }

    public interface ICommandActor<in TCommand, TResult> : ICommandActor where TCommand : ICommand<TResult>
    {
        Task<TResult> ExecuteAsync(TCommand command, TResult previousResult);
    }

    public interface ICommandChainActor<in TCommand, TResult> : ICommandChainActor where TCommand : ICommand<TResult>
    {
        // return true to stop after execution
        Task<CommandChainActorResult<TResult>> ExecuteAsync(TCommand command, TResult previousResult);
    }
}
