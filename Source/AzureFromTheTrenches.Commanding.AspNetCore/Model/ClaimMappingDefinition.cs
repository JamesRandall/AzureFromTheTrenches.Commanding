using System;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Model
{
    public class ClaimMappingDefinition
    {
        public string ClaimType { get; set; }

        public string PropertyName { get; set; }

        public StringComparison StringComparison { get; set; }
    }
}
