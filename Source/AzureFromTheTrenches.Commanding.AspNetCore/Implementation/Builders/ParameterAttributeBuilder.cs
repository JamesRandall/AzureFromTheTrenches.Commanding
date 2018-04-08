using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation.Builders
{
    class ParameterAttributeBuilder : IParameterAttributeBuilder
    {
        private readonly AttributeDefinition _attributeDefinition;

        public ParameterAttributeBuilder(AttributeDefinition attributeDefinition)
        {
            _attributeDefinition = attributeDefinition;
        }

        public INamedParameterAttributeBuilder Parameter(string name, object value)
        {
            _attributeDefinition.NamedParameters[name] = value;
            return this;
        }

        public IParameterAttributeBuilder Parameter(object value)
        {
            _attributeDefinition.UnnamedParameters.Add(value);
            return this;
        }
    }
}
