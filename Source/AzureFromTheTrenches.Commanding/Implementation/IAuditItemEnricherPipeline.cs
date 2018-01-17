using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal interface IAuditItemEnricherPipeline
    {
        void Enrich(Dictionary<string, string> properties, ICommand command, ICommandDispatchContext context);

        void AddEnricher<TAuditItemEnricher>() where TAuditItemEnricher : IAuditItemEnricher;
    }
}
