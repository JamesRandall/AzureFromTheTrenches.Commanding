namespace AzureFromTheTrenches.Commanding.Abstractions.Model
{
    /// <summary>
    /// Represents a command system result
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deferExecution">Was execution deferred</param>
        public CommandResult(bool deferExecution)
        {
            DeferExecution = deferExecution;
        }

        /// <summary>
        /// Was execution deferred - normally true when a command has been sent to a queue or some other async process
        /// </summary>
        public bool DeferExecution { get; }
    }

    /// <summary>
    /// Represents a command system result with a wrapped result from the handlers
    /// </summary>
    public sealed class CommandResult<TResult> : CommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">The result from the execution stage</param>
        /// <param name="deferExecution">Was execution deferred</param>
        public CommandResult(TResult result, bool deferExecution) : base(deferExecution)
        {
            Result = result;
        }

        /// <summary>
        /// The result of the command execution
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Implicit cast that auto-casts the command result to the wrapped result type.
        /// </summary>
        /// <param name="commandResult">The command result</param>
        public static implicit operator TResult(CommandResult<TResult> commandResult)
        {
            return commandResult.Result;
        }
    }
}
