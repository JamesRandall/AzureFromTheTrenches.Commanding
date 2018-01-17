using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class AuditItemEnricherPipeline : IAuditItemEnricherPipeline
    {
        private readonly Func<Type, IAuditItemEnricher> _enricherCreatorFunc;
        private readonly List<Type> _enrichers = new List<Type>();

        public AuditItemEnricherPipeline(Func<Type, IAuditItemEnricher> enricherCreatorFunc)
        {
            _enricherCreatorFunc = enricherCreatorFunc;
        }

        public void Enrich(Dictionary<string, string> properties, ICommand command, ICommandDispatchContext context)
        {
            foreach (Type enricherType in _enrichers)
            {
                IAuditItemEnricher enricher = _enricherCreatorFunc(enricherType);
                enricher.Enrich(properties, command, context);
            }
        }
        
        public void AddEnricher<TAuditItemEnricher>() where TAuditItemEnricher : IAuditItemEnricher
        {
            _enrichers.Add(typeof(TAuditItemEnricher));
        }
    }
}
