using System.Collections.Generic;

namespace AccidentalFish.Commanding
{
    public interface ICommandContextEnricher
    {
        IReadOnlyDictionary<string, object> GetAdditionalProperties(IReadOnlyDictionary<string, object> existingEnrichmentProperties);
    }
}
