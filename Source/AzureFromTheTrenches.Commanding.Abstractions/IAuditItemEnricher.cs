using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface IAuditItemEnricher
    {
        void Enrich(Dictionary<string, string> properties, ICommand command, ICommandDispatchContext context);
    }
}
