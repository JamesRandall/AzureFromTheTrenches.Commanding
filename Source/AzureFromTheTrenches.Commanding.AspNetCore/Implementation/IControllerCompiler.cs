using System;
using System.Collections.Generic;
using System.Reflection;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    interface IControllerCompiler
    {
        Assembly Compile(IReadOnlyCollection<ControllerDefinition> definitions, string outputNamespaceName);
    }
}
