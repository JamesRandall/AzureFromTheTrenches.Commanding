using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Context information used during command dispatch and execution to track and correlate commands
    /// </summary>
    public interface ICommandDispatchContext
    {
        /// <summary>
        /// The correlation ID for the current context
        /// </summary>
        string CorrelationId { get; }

        /// <summary>
        /// On first dispatch of a command (for example in a Web API controller) depth will be 0.
        /// If the handler for that command then dispatches another command depth will be 1. Etc.
        /// This is accurate unless a handler dispatches commands in parallel. I.e. if a handler
        /// dispatches a set of commands and awaits for them all as a batch e.g.
        /// Task.WhenAll(setOfDispatchTasks). In which case while the depth will be incremented
        /// and decremented reliably (it will unwind correctly to 0) it can no longer be used
        /// to ascertain command dispatch chain depth.
        /// 
        /// However depth > 0 is always an accurate way to ascertain whether or not we are in
        /// the root command which is the core frameworks usecase.
        /// </summary>
        int Depth { get; }

        /// <summary>
        /// Audit item enrichers are able to add properties into this dictionary so that they are
        /// available for audit across the dispatch process
        /// </summary>
        IReadOnlyDictionary<string, object> AdditionalProperties { get; }

        /// <summary>
        /// Increment the command depth tracker
        /// </summary>
        /// <returns>The depth</returns>
        int Increment();

        /// <summary>
        /// Decrement the command depth tracker
        /// </summary>
        /// <returns>The depth</returns>
        int Decrement();

        /// <summary>
        /// Return a copy of the context
        /// </summary>
        /// <returns>A new copy</returns>
        ICommandDispatchContext Copy();
    }
}
