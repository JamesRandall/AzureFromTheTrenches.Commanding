using System;
using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation.Compilation
{
    interface IControllerTemplateCompiler
    {
        Dictionary<string, Func<object, string>> CompileTemplates(IReadOnlyCollection<string> controllerNames);
    }
}
