using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation.Model
{
    internal class AugmentedControllerDefinition
    {
        private AugmentedControllerDefinition()
        {

        }

        public string Name { get; set; }

        public string Namespace { get; set; }

        public string Route { get; set; }

        public IReadOnlyCollection<AugmentedActionDefinition> Actions { get; set; }

        public static AugmentedControllerDefinition FromControllerDefinition(ControllerDefinition from, ClaimsMappingBuilder claimsMappingBuilder)
        {
            return new AugmentedControllerDefinition
            {
                Name = from.Name,
                Actions = from.Actions.Select(x => AugmentedActionDefinition.FromActionDefinition(x, claimsMappingBuilder)).ToArray(),
                Namespace = from.Namespace,
                Route = from.Route
            };
        }
    }
}
