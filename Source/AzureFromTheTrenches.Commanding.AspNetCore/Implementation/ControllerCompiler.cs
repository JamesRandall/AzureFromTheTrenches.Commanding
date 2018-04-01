using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.Implementation.Model;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    class ControllerCompiler : IControllerCompiler
    {
        private readonly IControllerTemplateCompiler _controllerTemplateCompiler;
        private readonly ISyntaxTreeCompiler _syntaxTreeCompiler;
        private readonly ClaimsMappingBuilder _claimsMappingBuilder;

        public ControllerCompiler(IControllerTemplateCompiler controllerTemplateCompiler,
            ISyntaxTreeCompiler syntaxTreeCompiler,
            ClaimsMappingBuilder claimsMappingBuilder)
        {
            _controllerTemplateCompiler = controllerTemplateCompiler;
            _syntaxTreeCompiler = syntaxTreeCompiler;
            _claimsMappingBuilder = claimsMappingBuilder;
        }

        public Assembly Compile(IReadOnlyCollection<ControllerDefinition> definitions, string outputNamespaceName)
        {
            var templates = _controllerTemplateCompiler.CompileTemplates(definitions.Select(x => x.Name).ToArray());
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();

            foreach (ControllerDefinition definition in definitions)
            {
                Func<object,string> template = templates[definition.Name];
                
                string controllerClassSource = template(AugmentedControllerDefinition.FromControllerDefinition(definition, _claimsMappingBuilder));

                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(controllerClassSource);
                syntaxTrees.Add(syntaxTree);
            }

            Assembly controllersAssembly = _syntaxTreeCompiler.CompileAssembly(string.Concat(outputNamespaceName, ".dll"), syntaxTrees);
            return controllersAssembly;
        }
    }
}
