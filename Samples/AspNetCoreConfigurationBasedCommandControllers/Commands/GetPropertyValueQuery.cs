using AspNetCoreConfigurationBasedCommandControllers.Commands.Responses;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AspNetCoreConfigurationBasedCommandControllers.Commands
{
    public class GetPropertyValueQuery : ICommand<PropertyValue>
    {
        public string Fqn { get; set; }
    }
}
