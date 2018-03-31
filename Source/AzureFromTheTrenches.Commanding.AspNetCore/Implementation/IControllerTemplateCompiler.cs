using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.AspNetCore.Templates;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    interface IControllerTemplateCompiler
    {
        Dictionary<string, ControllerTemplate> CompileTemplates(IReadOnlyCollection<string> controllerNames);
    }
}
