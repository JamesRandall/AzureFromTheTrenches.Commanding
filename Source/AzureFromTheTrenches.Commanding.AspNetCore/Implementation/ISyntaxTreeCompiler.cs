using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    internal interface ISyntaxTreeCompiler
    {
        Assembly CompileAssembly(string outputAssemblyName, IReadOnlyCollection<SyntaxTree> syntaxTrees);
    }
}
