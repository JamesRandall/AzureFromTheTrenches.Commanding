using System;
using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Model
{
    public class AttributeDefinition
    {
        public Type AttributeType { get; set; }

        public List<object> UnnamedParameters { get; set; } = new List<object>();

        public Dictionary<string, object> NamedParameters { get; set; } = new Dictionary<string, object>();

        public bool HasParameters => UnnamedParameters.Count > 0 || NamedParameters.Count > 0;
    }
}
