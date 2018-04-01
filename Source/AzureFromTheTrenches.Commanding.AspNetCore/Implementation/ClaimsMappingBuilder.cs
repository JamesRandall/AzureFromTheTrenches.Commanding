using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.Implementation.Model;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    internal class ClaimsMappingBuilder : IClaimsMappingBuilder
    {
        private readonly List<ClaimMappingDefinition> _claimMappingDefinitions = new List<ClaimMappingDefinition>();

        private readonly Dictionary<Type, CommandClaimMappingDefinition> _commandClaimMappingDefinitions = new Dictionary<Type, CommandClaimMappingDefinition>();

        public IClaimsMappingBuilder MapClaimToPropertyName(string claimType, string propertyName, StringComparison nameComparisonType= StringComparison.InvariantCultureIgnoreCase)
        {
            _claimMappingDefinitions.Add(new ClaimMappingDefinition
            {
                ClaimType = claimType,
                PropertyName = propertyName,
                StringComparison = nameComparisonType
            });
            return this;
        }

        public IClaimsMappingBuilder MapClaimToCommandProperty<TCommand>(string claimType, Expression<Func<TCommand, object>> getProperty)
        {
            _commandClaimMappingDefinitions.Add(typeof(TCommand), new CommandClaimMappingDefinition
            {
                ClaimType = claimType,
                CommandType = typeof(TCommand),
                PropertyInfo = (PropertyInfo)((MemberExpression)getProperty.Body).Member
            });
            return this;
        }

        public IReadOnlyCollection<ClaimMappingDefinition> ClaimMappingDefinitions => _claimMappingDefinitions;

        public IDictionary<Type, CommandClaimMappingDefinition> CommandClaimMappingDefinitions => _commandClaimMappingDefinitions;

        public IReadOnlyCollection<ClaimMapping> GetMappingsForCommandType(Type commandType)
        {
            return new ClaimMapping[0];
        }
    }
}
