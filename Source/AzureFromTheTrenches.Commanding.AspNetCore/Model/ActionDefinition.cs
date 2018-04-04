using System;
using System.Net.Http;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Model
{
    public class ActionDefinition
    {
        public HttpMethod Verb { get; set; }

        public string Route { get; set; }

        public Type CommandType { get; set; }

        public Type ResultType { get; set; }

        public Type BindingAttributeType { get; set; }

        public bool HasProperties => CommandType.GetProperties().Length > 0;
    }
}
