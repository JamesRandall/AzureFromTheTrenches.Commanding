using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public interface IParameterAttributeBuilder: INamedParameterAttributeBuilder
    {
        IParameterAttributeBuilder Parameter(object value);
    }
}
