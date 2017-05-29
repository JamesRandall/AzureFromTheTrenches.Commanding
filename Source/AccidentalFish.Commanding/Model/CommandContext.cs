using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AccidentalFish.Commanding.Model
{
    internal sealed class CommandContext : ICommandContext
    {
        private int _depth;

        public CommandContext(string correlationId, IReadOnlyDictionary<string, object> additionalProperties)
        {
            CorrelationId = correlationId;
            AdditionalProperties = additionalProperties;
            _depth = 0;
        }

        public string CorrelationId { get; }

        public int Depth => _depth;

        public IReadOnlyDictionary<string, object> AdditionalProperties { get; }

        public int Increment()
        {
            return Interlocked.Increment(ref _depth);
        }

        public int Decrement()
        {
            return Interlocked.Decrement(ref _depth);
        }

        public ICommandContext Copy()
        {
            IReadOnlyDictionary<string, object> copiedDictionary = AdditionalProperties.ToDictionary(x => x.Key, x => x.Value);
            CommandContext copy = new CommandContext(CorrelationId, copiedDictionary) {_depth = _depth};
            return copy;
        }
    }
}
