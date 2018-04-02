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
        private readonly bool _reusePreviousCompile;
        private static readonly object CompileLock = new object();

        public ControllerCompiler(IControllerTemplateCompiler controllerTemplateCompiler,
            ISyntaxTreeCompiler syntaxTreeCompiler,
            Action<string> constructedCodeLogger,
            bool reusePreviousCompile)
        {
            _controllerTemplateCompiler = controllerTemplateCompiler;
            _syntaxTreeCompiler = syntaxTreeCompiler;
            _constructedCodeLogger = constructedCodeLogger;
            _reusePreviousCompile = reusePreviousCompile;
        }

        public Assembly Compile(IReadOnlyCollection<ControllerDefinition> definitions, string outputNamespaceName)
        {
            lock (CompileLock)
            {
                // if this isn't set to true and the compile occurs again then if the same namespace is used
                // an exception will be thrown.
                // this should only occur in acceptance test scenarios with TestServer where the MVC app will be
                // stood up multiple times (and in parallel) in the same process / app domain
                if (_reusePreviousCompile)
                {
                    Assembly existingAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(x => x.FullName == $"{outputNamespaceName}.dll, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
                    if (existingAssembly != null)
                    {
                        return existingAssembly;
                    }
                }
                var templates = _controllerTemplateCompiler.CompileTemplates(definitions.Select(x => x.Name).ToArray());
                List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();

                foreach (ControllerDefinition definition in definitions)
                {
                    Func<object, string> template = templates[definition.Name];

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
}
