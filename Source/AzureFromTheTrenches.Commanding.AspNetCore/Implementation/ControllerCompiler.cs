using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.Templates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    class ControllerCompiler : IControllerCompiler
    {
        private readonly IControllerTemplateProvider _controllerTemplateProvider;
        private readonly ISyntaxTreeCompiler _syntaxTreeCompiler;

        public ControllerCompiler(IControllerTemplateProvider controllerTemplateProvider,
            ISyntaxTreeCompiler syntaxTreeCompiler)
        {
            _controllerTemplateProvider = controllerTemplateProvider;
            _syntaxTreeCompiler = syntaxTreeCompiler;
        }

        public void Compile(IReadOnlyCollection<ControllerDefinition> definitions, string outputNamespaceName)
        {
            var templates = _controllerTemplateProvider.CompileTemplates(definitions.Select(x => x.Name).ToArray());
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();

            foreach (ControllerDefinition definition in definitions)
            {
                ControllerTemplate template = templates[definition.Name];
                template.Model = definition;
                template.ExecuteAsync().Wait();
                string controllerClassSource = template.Output.ToString();

                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(controllerClassSource);
                syntaxTrees.Add(syntaxTree);
            }

            Assembly controllersAssembly = _syntaxTreeCompiler.CompileAssembly(string.Concat(outputNamespaceName, ".dll"), syntaxTrees);
        }
    }
}
