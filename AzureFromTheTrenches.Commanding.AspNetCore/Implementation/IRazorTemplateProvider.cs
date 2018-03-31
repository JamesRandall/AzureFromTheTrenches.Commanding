using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.AspNetCore.Templates;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    interface IRazorTemplateProvider
    {
        Dictionary<string, ControllerRazorTemplate> CompileTemplates(IReadOnlyCollection<string> controllerNames);
    }
}
