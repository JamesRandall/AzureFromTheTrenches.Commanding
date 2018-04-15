using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding
{
    /// <summary>
    /// Utility class that wraps a function as a command dispatch context enricher
    /// </summary>
    public class FunctionWrapperCommandDispatchContextEnricher : ICommandDispatchContextEnricher
    {
        private readonly Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>> _enrichmentFunc;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="func">The function to wrap</param>
        public FunctionWrapperCommandDispatchContextEnricher(Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>> func)
        {
            _enrichmentFunc = func;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, object> GetAdditionalProperties(IReadOnlyDictionary<string, object> existingEnrichmentProperties)
        {
            return _enrichmentFunc(existingEnrichmentProperties);
        }
    }
}
