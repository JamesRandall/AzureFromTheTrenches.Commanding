using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation.Model
{
    internal class AugmentedActionDefinition
    {
        private AugmentedActionDefinition()
        {

        }

        public HttpMethod Verb { get; set; }

        public string Route { get; set; }

        public Type CommandType { get; set; }

        public Type ResultType { get; set; }

        public Type BindingAttributeType { get; set; }

        public IReadOnlyCollection<ClaimMapping> ClaimMappings { get; set; }

        public static AugmentedActionDefinition FromActionDefinition(ActionDefinition from, ClaimsMappingBuilder claimsBuilder)
        {
            return new AugmentedActionDefinition
            {
                Route = from.Route,
                BindingAttributeType = from.BindingAttributeType,
                CommandType = from.CommandType,
                ResultType = from.ResultType,
                Verb = from.Verb
            };
        }
    }
}
