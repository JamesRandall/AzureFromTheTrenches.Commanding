using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.AspNetCore.Templates;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    interface IControllerTemplateCompiler
    {
        Dictionary<string, Func<object, string>> CompileTemplates(IReadOnlyCollection<string> controllerNames);
    }
}
