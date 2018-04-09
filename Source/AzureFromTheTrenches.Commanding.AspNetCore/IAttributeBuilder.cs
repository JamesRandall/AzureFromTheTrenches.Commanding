using System;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public interface IAttributeBuilder
    {
        IAttributeBuilder Attribute<TAttribute>() where TAttribute : Attribute;

        IAttributeBuilder Attribute<TAttribute>(Action<IParameterAttributeBuilder> parameterAttributeBuilder)
            where TAttribute : Attribute;
    }
}
