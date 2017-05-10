using System.Threading.Tasks;

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
        /// <typeparam name="T">The type of the command</typeparam>
        /// <param name="command">The command</param>
        /// <returns>True if the command was executed, false if the dispatcher utilises a deferred execution model</returns>
        Task<bool> DispatchAsync<T>(T command) where T : class;
    }
}
