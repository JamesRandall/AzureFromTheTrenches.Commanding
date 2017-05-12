using System.Threading.Tasks;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding
{
    /// <summary>
    /// The primary dispatch mechanism for commands. If a deferred dispatcher is not registered then the command will also be run at the point
    /// of dispatch.
    /// 
    /// This allows for configuration based behaviour modification.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Dispatches a command and returns true if the command was immediately executed
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="command">The command</param>
        /// <returns>Result of the command</returns>
        Task<CommandResult<TResult>> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : class;

        /// <summary>
        /// The dispatchers associated executer
        /// </summary>
        ICommandExecuter AssociatedExecuter { get; }
    }
}
