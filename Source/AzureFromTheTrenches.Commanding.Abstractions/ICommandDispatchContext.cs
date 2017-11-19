using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandDispatchContext
    {
        string CorrelationId { get; }

        int Depth { get; }

        IReadOnlyDictionary<string, object> AdditionalProperties { get; }

        int Increment();

        int Decrement();

        ICommandDispatchContext Copy();
    }
}
