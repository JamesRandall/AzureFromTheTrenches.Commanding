using System.Collections.Generic;

namespace AccidentalFish.Commanding
{
    public interface ICommandContext
    {
        string CorrelationId { get; }

        int Depth { get; }

        IReadOnlyDictionary<string, object> AdditionalProperties { get; }

        int Increment();

        int Decrement();

        ICommandContext Copy();
    }
}
