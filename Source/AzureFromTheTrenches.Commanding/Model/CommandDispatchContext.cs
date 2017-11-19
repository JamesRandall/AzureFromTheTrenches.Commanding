using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Model
{
    internal sealed class CommandDispatchContext : ICommandDispatchContext
    {
        private int _depth;

        public CommandDispatchContext(string correlationId, IReadOnlyDictionary<string, object> additionalProperties)
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

        public ICommandDispatchContext Copy()
        {
            IReadOnlyDictionary<string, object> copiedDictionary = AdditionalProperties.ToDictionary(x => x.Key, x => x.Value);
            CommandDispatchContext copy = new CommandDispatchContext(CorrelationId, copiedDictionary) { _depth = _depth };
            return copy;
        }
    }
}