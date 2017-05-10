using System.Threading.Tasks;

namespace AccidentalFish.Commanding
{
    /// <summary>
    /// Implementations of this interface execute commands immediately using the registered set of actors
    /// </summary>
    public interface ICommandExecuter
    {
        /// <summary>
        /// Executes the given command
        /// </summary>
        /// <typeparam name="T">The type of the command</typeparam>
        /// <param name="command">The command to execute</param>
        /// <returns>Awaitable task</returns>
        Task ExecuteAsync<T>(T command) where T : class;
    }
}
