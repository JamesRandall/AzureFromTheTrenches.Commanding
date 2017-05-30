using System;
using System.Collections.Generic;
using System.Linq;

namespace AccidentalFish.Commanding.Implementation
{
    internal class CommandDispatchContextEnrichment : ICommandDispatchContextEnrichment
    {
        private readonly List<ICommandDispatchContextEnricher> _enrichers;

        public CommandDispatchContextEnrichment(IEnumerable<ICommandDispatchContextEnricher> enrichers)
        {
            _enrichers = new List<ICommandDispatchContextEnricher>(enrichers);
        }

        public CommandDispatchContextEnrichment(IEnumerable<Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>> enricherFuncs)
        {
            _enrichers = new List<ICommandDispatchContextEnricher>(enricherFuncs.Select(x => new FunctionWrapperCommandDispatchContextEnricher(x))).ToList();
        }

        public IReadOnlyDictionary<string, object> GetAdditionalProperties()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (ICommandDispatchContextEnricher enricher in _enrichers)
            {
                IReadOnlyDictionary<string, object> newProperties = enricher.GetAdditionalProperties(result);
                foreach(KeyValuePair<string,object> kvp in newProperties)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            return result;
        }

        public void AddEnrichers(IEnumerable<ICommandDispatchContextEnricher> enrichers)
        {
            _enrichers.AddRange(enrichers);
        }

        public void AddEnrichers(IEnumerable<Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>> enricherFuncs)
        {
            _enrichers.AddRange(enricherFuncs.Select(x => new FunctionWrapperCommandDispatchContextEnricher(x)));
        }
    }
}
