using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.Implementation.Model;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    internal class ClaimsMappingBuilder : IClaimsMappingBuilder
    {
        private readonly Dictionary<Type, IReadOnlyCollection<ClaimMapping>> _mappedCommands = new Dictionary<Type, IReadOnlyCollection<ClaimMapping>>();

        private readonly List<ClaimMappingDefinition> _claimMappingDefinitions = new List<ClaimMappingDefinition>();

        private readonly Dictionary<Type, List<CommandClaimMappingDefinition>> _commandClaimMappingDefinitions = new Dictionary<Type, List<CommandClaimMappingDefinition>>();

        public IClaimsMappingBuilder MapClaimToPropertyName(string claimType, string propertyName)
        {
            _claimMappingDefinitions.Add(new ClaimMappingDefinition
            {
                ClaimType = claimType,
                PropertyName = propertyName
            });
            return this;
        }

        public IClaimsMappingBuilder MapClaimToCommandProperty<TCommand>(string claimType, Expression<Func<TCommand, object>> getProperty)
        {
            if (!_commandClaimMappingDefinitions.TryGetValue(typeof(TCommand), out List<CommandClaimMappingDefinition> list))
            {
                list = new List<CommandClaimMappingDefinition>();
                _commandClaimMappingDefinitions[typeof(TCommand)] = list;
            }

            list.Add(new CommandClaimMappingDefinition
            {
                ClaimType = claimType,
                CommandType = typeof(TCommand),
                PropertyInfo = (PropertyInfo)((MemberExpression)getProperty.Body).Member
            });
            return this;
        }

        internal IReadOnlyCollection<ClaimMapping> GetMappingsForCommandType(Type commandType)
        {
            if (_mappedCommands.TryGetValue(commandType, out IReadOnlyCollection<ClaimMapping> cachedResult))
            {
                return cachedResult;
            }

            Dictionary<string, PropertyInfo> commandProperties = commandType.GetProperties().ToDictionary(x => x.Name, x => x);
            Dictionary<string, ClaimMapping> mappingsByPropertyName = new Dictionary<string, ClaimMapping>();
            
            // we do the generic non-command specific claim to property mappings first as these are overridden by any command
            // specific mappings
            foreach (ClaimMappingDefinition genericMappingDefinition in _claimMappingDefinitions)
            {
                if (commandProperties.TryGetValue(genericMappingDefinition.PropertyName, out PropertyInfo property))
                {
                    mappingsByPropertyName[property.Name] = new ClaimMapping
                    {
                        FromClaimType = genericMappingDefinition.ClaimType,
                        ToPropertyName = property.Name,
                        ToPropertyType = property.PropertyType.FullName
                    };
                }
            }

            // then we do command specific mappings, these override generic mappings
            if (_commandClaimMappingDefinitions.TryGetValue(commandType, out List<CommandClaimMappingDefinition> commandClaimMappingDefinitions))
            {
                foreach (CommandClaimMappingDefinition definition in commandClaimMappingDefinitions)
                {
                    mappingsByPropertyName[definition.PropertyInfo.Name] = new ClaimMapping
                    {
                        FromClaimType = definition.ClaimType,
                        ToPropertyName = definition.PropertyInfo.Name,
                        ToPropertyType = definition.PropertyInfo.PropertyType.FullName
                    };
                }
            }

            return mappingsByPropertyName.Values;
        }
    }
}
