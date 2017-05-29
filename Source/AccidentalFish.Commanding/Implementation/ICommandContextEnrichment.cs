using System;
using System.Collections.Generic;

namespace AccidentalFish.Commanding.Implementation
{
    internal interface ICommandContextEnrichment
    {
        IReadOnlyDictionary<string, object> GetAdditionalProperties();
        void AddEnrichers(IEnumerable<ICommandContextEnricher> enrichers);
        void AddEnrichers(IEnumerable<Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>> enricherFuncs);
    }
}
