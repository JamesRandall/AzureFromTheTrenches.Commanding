namespace AzureFromTheTrenches.Commanding.Abstractions.Model
{
    /// <summary>
    /// Returned from pipeline aware command handlers to signify if the command execution pipeline should be halted
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class PipelineAwareCommandHandlerResult<TResult>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="shouldStop">Should the execution pipeline be halted</param>
        /// <param name="result">The result of the handler</param>
        public PipelineAwareCommandHandlerResult(bool shouldStop, TResult result)
        {
            ShouldStop = shouldStop;
            Result = result;
        }

        /// <summary>
        /// Should the execution pipeline be halted
        /// </summary>
        public bool ShouldStop { get; }

        /// <summary>
        /// The result of the handler
        /// </summary>
        public TResult Result { get; }
    }
}
