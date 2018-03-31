using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    public class ControllerDefinition
    {
        public string Name { get; set; }

        public string Route { get; set; }

        public IReadOnlyCollection<ActionDefinition> Actions { get; set; }
    }
}
