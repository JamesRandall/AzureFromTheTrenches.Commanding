using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Implementations of this interface can add additional properties to audit items
    /// </summary>
    public interface IAuditItemEnricher
    {
        /// <summary>
        /// Should enrich the properties dictionary with any additional required properties
        /// </summary>
        /// <param name="properties">Audit item properties</param>
        /// <param name="command">The command being audited</param>
        /// <param name="context">The dispatch context for the command being audited</param>
        void Enrich(Dictionary<string, string> properties, ICommand command, ICommandDispatchContext context);
    }
}
