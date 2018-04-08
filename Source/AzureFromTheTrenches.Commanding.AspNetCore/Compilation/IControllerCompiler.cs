using System.Collections.Generic;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Compilation
{
    interface IControllerCompiler
    {
        Assembly Compile(IReadOnlyCollection<ControllerDefinition> definitions, string outputNamespaceName);
    }
}
