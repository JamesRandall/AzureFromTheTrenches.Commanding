using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions.Model;

namespace AccidentalFish.Commanding.Abstractions
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
        /// Dispatches a command and returns the response
        /// </summary>
        /// <typeparam name="TResult">Result of the command</typeparam>
        /// <param name="command">The command</param>
        /// <returns>Command result</returns>
        Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command);

        /// <summary>
        /// The dispatchers associated executer
        /// </summary>
        ICommandExecuter AssociatedExecuter { get; }
    }
}
