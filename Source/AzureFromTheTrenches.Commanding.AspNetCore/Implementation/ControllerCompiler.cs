using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    class ControllerCompiler : IControllerCompiler
    {
        private readonly IControllerTemplateCompiler _controllerTemplateCompiler;
        private readonly ISyntaxTreeCompiler _syntaxTreeCompiler;
        private readonly Action<string> _constructedCodeLogger;

        public ControllerCompiler(IControllerTemplateCompiler controllerTemplateCompiler,
            ISyntaxTreeCompiler syntaxTreeCompiler, Action<string> constructedCodeLogger)
        {
            _controllerTemplateCompiler = controllerTemplateCompiler;
            _syntaxTreeCompiler = syntaxTreeCompiler;
            _constructedCodeLogger = constructedCodeLogger;
        }

        public Assembly Compile(IReadOnlyCollection<ControllerDefinition> definitions, string outputNamespaceName)
        {
            var templates = _controllerTemplateCompiler.CompileTemplates(definitions.Select(x => x.Name).ToArray());
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();

            foreach (ControllerDefinition definition in definitions)
            {
                Func<object,string> template = templates[definition.Name];
                
                string controllerClassSource = template(definition);

                _constructedCodeLogger?.Invoke(controllerClassSource);

                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(controllerClassSource);
                syntaxTrees.Add(syntaxTree);
            }

            Assembly controllersAssembly = _syntaxTreeCompiler.CompileAssembly(string.Concat(outputNamespaceName, ".dll"), syntaxTrees);
            return controllersAssembly;
        }
    }
}
