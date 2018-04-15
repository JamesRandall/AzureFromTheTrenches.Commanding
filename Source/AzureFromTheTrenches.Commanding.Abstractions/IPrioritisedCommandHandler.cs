using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Associates a command handler (type) with it's priority / order in the pipeline.
    /// </summary>
    public interface IPrioritisedCommandHandler
    {
        /// <summary>
        /// The priority of the command
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// The type of the handler
        /// </summary>
        Type CommandHandlerType { get; }
    }
}
