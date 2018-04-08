using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public interface INamedParameterAttributeBuilder
    {
        INamedParameterAttributeBuilder Parameter(string name, object value);
    }
}
