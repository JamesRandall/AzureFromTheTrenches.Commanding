using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.Templates;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    class ControllerTemplateCompiler : IControllerTemplateCompiler
    {
        private readonly string _outputNamespaceName;
        private readonly string _outputAssemblyName;
        private readonly Func<string, Stream> _externalTemplateProvider;
        private readonly ISyntaxTreeCompiler _syntaxTreeCompiler;

        public ControllerTemplateCompiler(string outputNamespaceName,
            Func<string, Stream> externalTemplateProvider,
            ISyntaxTreeCompiler syntaxTreeCompiler)
        {
            _outputNamespaceName = outputNamespaceName;
            _outputAssemblyName = $"{outputNamespaceName}.dll";
            _externalTemplateProvider = externalTemplateProvider;
            _syntaxTreeCompiler = syntaxTreeCompiler;
        }

        public Dictionary<string, ControllerTemplate> CompileTemplates(IReadOnlyCollection<string> controllerNames)
        {
            var templateEngine = CreateRazorTemplateEngine();

            var controllerSyntaxTrees = GetControllerSyntaxTrees(templateEngine, controllerNames);

            Assembly assembly = CompileAssembly(controllerSyntaxTrees);
            
            return GetCompiledTemplateInstances(assembly, controllerNames);
        }

        private List<SyntaxTree> GetControllerSyntaxTrees(RazorTemplateEngine templateEngine, IReadOnlyCollection<string> controllerNames)
        {
            List<SyntaxTree> controllerSyntaxTrees = new List<SyntaxTree>(new[] {GetDefaultTemplateSyntaxTree(templateEngine)});

            foreach (string controllerName in controllerNames)
            {
                if (_externalTemplateProvider != null)
                {
                    using (Stream stream = _externalTemplateProvider(controllerName))
                    {
                        if (stream != null)
                        {
                            controllerSyntaxTrees.Add(GetSyntaxTreeFromStream(templateEngine, stream,
                                string.Concat(controllerName, "Template")));
                        }
                    }
                }
            }

            return controllerSyntaxTrees;
        }

        private Dictionary<string, ControllerTemplate> GetCompiledTemplateInstances(Assembly assembly, IReadOnlyCollection<string> controllerNames)
        {
            Type defaultType = assembly.GetType($"{_outputNamespaceName}.DefaultTemplate");
            ControllerTemplate defaultTemplate = (ControllerTemplate)Activator.CreateInstance(defaultType);
            Dictionary<string, ControllerTemplate> result = new Dictionary<string, ControllerTemplate>();
            foreach (string controllerName in controllerNames)
            {
                ControllerTemplate controllerTemplate = defaultTemplate;
                Type specificControllerType = assembly.GetType($"{_outputAssemblyName}.{controllerName}Template");
                if (specificControllerType != null)
                {
                    controllerTemplate = (ControllerTemplate)Activator.CreateInstance(specificControllerType);
                }

                result[controllerName] = controllerTemplate;
            }

            return result;
        }

        private Assembly CompileAssembly(IReadOnlyCollection<SyntaxTree> syntaxTrees)
        {
            return _syntaxTreeCompiler.CompileAssembly(_outputAssemblyName, syntaxTrees);
        }

        private RazorTemplateEngine CreateRazorTemplateEngine()
        {
            RazorEngine engine = RazorEngine.Create(cfg =>
            {
                InheritsDirective.Register(cfg);
                cfg.SetNamespace(_outputNamespaceName);
                cfg.Build();
            });
            RazorProject project = RazorProject.Create(".");
            RazorTemplateEngine templateEngine = new RazorTemplateEngine(engine, project);
            return templateEngine;
        }

        private SyntaxTree GetDefaultTemplateSyntaxTree(RazorTemplateEngine templateEngine)
        {
            using (Stream stream =
                GetType().Assembly.GetManifestResourceStream(
                    "AzureFromTheTrenches.Commanding.AspNetCore.Templates.DefaultController.cstpl"))
            {
                return GetSyntaxTreeFromStream(templateEngine, stream, "DefaultTemplate");
            }
        }

        private SyntaxTree GetSyntaxTreeFromStream(RazorTemplateEngine templateEngine, Stream stream, string className)
        {
            RazorCodeDocument doc = RazorCodeDocument.Create(RazorSourceDocument.ReadFrom(stream, "controller.cstpl"));
            RazorCSharpDocument csharp = templateEngine.GenerateCode(doc);
            string generatedCode = csharp.GeneratedCode;
            // TODO: Can't find the right hook to do this on Core 2.0 (it's a parameter on templateEngine.GenerateCode(...) on 1.1)
            generatedCode = generatedCode.Replace("public class Template : ", $"public class {className} : ");
            return CSharpSyntaxTree.ParseText(generatedCode);
        }
    }
}
