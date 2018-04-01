using System;
using AspNetCoreConfigurationBasedCommandControllers.Commands.Responses;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AspNetCoreConfigurationBasedCommandControllers.Commands
{
    public class UpdatePropertyValueCommand : PropertyValue, ICommand
    {
        [SecurityProperty]
        public Guid UserId { get; set; }
    }
}
