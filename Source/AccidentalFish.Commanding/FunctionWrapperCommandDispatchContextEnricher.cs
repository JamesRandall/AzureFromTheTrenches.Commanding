using System;
using System.Collections.Generic;

namespace AccidentalFish.Commanding
{
    public class FunctionWrapperCommandDispatchContextEnricher : ICommandDispatchContextEnricher
    {
        private readonly Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>> _enrichmentFunc;

        public FunctionWrapperCommandDispatchContextEnricher(Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>> func)
        {
            _enrichmentFunc = func;
        }

        public IReadOnlyDictionary<string, object> GetAdditionalProperties(IReadOnlyDictionary<string, object> existingEnrichmentProperties)
        {
            return _enrichmentFunc(existingEnrichmentProperties);
        }
    }
}
