using AspNetCoreConfigurationBasedCommandControllers.Commands.Responses;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspNetCoreConfigurationBasedCommandControllers.Commands
{
    public class GetPropertyValueQuery : ICommand<PropertyValue>
    {
        [SecurityProperty]
        public string UserId { get; set; }

        public string Fqn { get; set; }
    }
}
