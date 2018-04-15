namespace AzureFromTheTrenches.Commanding.Abstractions.Model
{
    /// <summary>
    /// Largely invisible to the consumer used in some code scenarios to treat commands with results and commands without results
    /// as being the same by wrapping a command in this decorator that has a NoResult result
    /// </summary>
    public class NoResultCommandWrapper : ICommand<NoResult>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">The underlying command with no result</param>
        public NoResultCommandWrapper(ICommand command)
        {
            Command = command;
        }

        /// <summary>
        /// The underlying (resultless) command
        /// </summary>
        public ICommand Command { get; }        
    }
}
