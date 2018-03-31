using System.Collections.Generic;
using System.Linq;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    class ControllerCompiler : IControllerCompiler
    {
        private readonly IRazorTemplateProvider _razorTemplateProvider;

        public ControllerCompiler(IRazorTemplateProvider razorTemplateProvider)
        {
            _razorTemplateProvider = razorTemplateProvider;
        }

        public void Compile(IReadOnlyCollection<ControllerDefinition> definitions)
        {
            _razorTemplateProvider.CompileTemplates(definitions.Select(x => x.Name).ToArray());
        }
    }
}
