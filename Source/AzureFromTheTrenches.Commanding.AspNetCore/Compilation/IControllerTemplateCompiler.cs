using System;
using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Compilation
{
    interface IControllerTemplateCompiler
    {
        Dictionary<string, Func<object, string>> CompileTemplates(IReadOnlyCollection<string> controllerNames);
    }
}
