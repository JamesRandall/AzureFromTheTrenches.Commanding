using System.Threading.Tasks;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding
{
    // NOTE: Both ICommandActorBase and ICommandActorResultBase are only used to provide generic type safety checks
    // and keep the command interface clean
    
    public interface ICommandActorBase<in TCommand> where TCommand : class
    {

    }

    public interface ICommandActorResultBase<in TCommand, TResult>  : ICommandActorBase<TCommand> where TCommand : class
    {

    }

    public interface ICommandActor<in TCommand, TResult> : ICommandActorResultBase<TCommand, TResult> where TCommand : class
    {
        Task<TResult> ExecuteAsync(TCommand command, TResult previousResult);
    }

    public interface ICommandChainActor<in TCommand, TResult> : ICommandActorResultBase<TCommand, TResult> where TCommand : class
    {
        // return true to stop after execution
        Task<CommandChainActorResult<TResult>> ExecuteAsync(TCommand command, TResult previousResult);
    }
}
