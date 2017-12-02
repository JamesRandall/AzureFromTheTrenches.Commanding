using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandDispatchContext
    {
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

        IReadOnlyDictionary<string, object> AdditionalProperties { get; }

        int Increment();

        int Decrement();

        ICommandDispatchContext Copy();
    }
}
