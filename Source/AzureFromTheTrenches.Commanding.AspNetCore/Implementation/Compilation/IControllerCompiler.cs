using System.Collections.Generic;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation.Compilation
{
    interface IControllerCompiler
    {
        Assembly Compile(IReadOnlyCollection<ControllerDefinition> definitions, string outputNamespaceName);
    }
}
