using System.Collections.Generic;

namespace AccidentalFish.Commanding
{
    public interface ICommandDispatchContextEnricher
    {
        /// <summary>
        /// Get additional properties to attach to a command dispatch context.
        /// Although it's tempting to think this should be supplied a command the context wraps potentially multiple command
        /// dispatches.
        /// </summary>
        /// <param name="existingEnrichmentProperties">The existing properties on the context applied by previous enrichers</param>
        /// <returns>The set of properties this enricher wants to attach</returns>
        IReadOnlyDictionary<string, object> GetAdditionalProperties(IReadOnlyDictionary<string, object> existingEnrichmentProperties);
    }
}
