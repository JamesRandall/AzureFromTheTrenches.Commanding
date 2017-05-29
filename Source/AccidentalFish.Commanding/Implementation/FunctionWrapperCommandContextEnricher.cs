using System;
using System.Collections.Generic;

namespace AccidentalFish.Commanding.Implementation
{
    class FunctionWrapperCommandContextEnricher : ICommandContextEnricher
    {
        private Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>> _enrichmentFunc;

        public FunctionWrapperCommandContextEnricher(Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>> func)
        {
            _enrichmentFunc = func;
        }

        public IReadOnlyDictionary<string, object> GetAdditionalProperties(IReadOnlyDictionary<string, object> existingEnrichmentProperties)
        {
            return _enrichmentFunc(existingEnrichmentProperties);
        }
    }
}
