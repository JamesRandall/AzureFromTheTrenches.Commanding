using System;
using System.Collections.Generic;
using System.Linq;

namespace AccidentalFish.Commanding.Implementation
{
    internal class CommandContextEnrichment : ICommandContextEnrichment
    {
        private readonly List<ICommandContextEnricher> _enrichers;

        public CommandContextEnrichment(IEnumerable<ICommandContextEnricher> enrichers)
        {
            _enrichers = new List<ICommandContextEnricher>(enrichers);
        }

        public CommandContextEnrichment(IEnumerable<Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>> enricherFuncs)
        {
            _enrichers = new List<ICommandContextEnricher>(enricherFuncs.Select(x => new FunctionWrapperCommandContextEnricher(x))).ToList();
        }

        public IReadOnlyDictionary<string, object> GetAdditionalProperties()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (ICommandContextEnricher enricher in _enrichers)
            {
                IReadOnlyDictionary<string, object> newProperties = enricher.GetAdditionalProperties(result);
                foreach(KeyValuePair<string,object> kvp in newProperties)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            return result;
        }

        public void AddEnrichers(IEnumerable<ICommandContextEnricher> enrichers)
        {
            _enrichers.AddRange(enrichers);
        }

        public void AddEnrichers(IEnumerable<Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>> enricherFuncs)
        {
            _enrichers.AddRange(enricherFuncs.Select(x => new FunctionWrapperCommandContextEnricher(x)));
        }
    }
}
