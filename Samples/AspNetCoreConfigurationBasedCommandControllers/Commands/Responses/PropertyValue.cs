using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreConfigurationBasedCommandControllers.Commands.Responses
{
    public class PropertyValue
    {
        public string PropertyFqn { get; set; }

        public string Value { get; set; }
    }
}
