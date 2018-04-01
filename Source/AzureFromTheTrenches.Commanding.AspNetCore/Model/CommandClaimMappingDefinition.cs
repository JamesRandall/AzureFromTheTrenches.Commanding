using System;
using System.Reflection;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Model
{
    public class CommandClaimMappingDefinition
    {
        public string ClaimType { get; set; }

        public Type CommandType { get; set; }

        public PropertyInfo PropertyInfo { get; set; }
    }
}
