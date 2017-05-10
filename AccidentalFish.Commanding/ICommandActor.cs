using System.Threading.Tasks;

namespace AccidentalFish.Commanding
{
    public interface ICommandActorBase<in T> where T : class
    {

    }

    public interface ICommandActor<in T> : ICommandActorBase<T> where T : class
    {
        Task ExecuteAsync(T command);
    }

    public interface ICommandChainActor<in T> : ICommandActorBase<T> where T : class
    {
        // return true to stop after execution
        Task<bool> ExecuteAsync(T command);
    }
}
