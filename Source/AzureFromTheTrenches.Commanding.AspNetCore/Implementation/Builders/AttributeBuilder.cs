using System;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation.Builders
{
    internal class AttributeBuilder: IAttributeBuilder
    {
        private readonly AbstractAttributableDefinition _attributableDefinition;

        public AttributeBuilder(AbstractAttributableDefinition attributableDefinition)
        {
            _attributableDefinition = attributableDefinition;
        }

        public IAttributeBuilder Attribute<TAttribute>(Action<IParameterAttributeBuilder> parameterAttributeBuilder) where TAttribute : Attribute
        {
            AttributeDefinition definition = new AttributeDefinition()
            {
                AttributeType = typeof(TAttribute)
            };
            parameterAttributeBuilder(new ParameterAttributeBuilder(definition));
            _attributableDefinition.Attributes.Add(definition);
            return this;
        }
    }
}
