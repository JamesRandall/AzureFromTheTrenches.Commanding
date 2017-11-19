using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal interface ICommandDispatchContextEnrichment
    {
        IReadOnlyDictionary<string, object> GetAdditionalProperties();
        void AddEnrichers(IEnumerable<ICommandDispatchContextEnricher> enrichers);
        void AddEnrichers(IEnumerable<Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>> enricherFuncs);
    }
}
