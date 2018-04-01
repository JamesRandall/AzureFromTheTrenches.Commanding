using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AspNetCoreConfigurationBasedCommandControllers.Commands.Responses
{
    public class PropertyValue
    {
        [SecurityProperty]
        public Guid UserId { get; set; }

        public string PropertyFqn { get; set; }

        public string Value { get; set; }
    }
}
