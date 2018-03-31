using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AzureFromTheTrenches.Commanding.AspNetCore.Templates;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    class RazorTemplateProvider : IRazorTemplateProvider
    {
        private readonly string _outputNamespaceName;
        private readonly string _outputAssemblyName;
        private readonly Func<string, Stream> _externalTemplateProvider;
        
        public RazorTemplateProvider(string outputNamespaceName,
            Func<string, Stream> externalTemplateProvider)
        {
            _outputNamespaceName = outputNamespaceName;
            _outputAssemblyName = $"{outputNamespaceName}.dll";
            _externalTemplateProvider = externalTemplateProvider;
        }

        public Dictionary<string, ControllerRazorTemplate> CompileTemplates(IReadOnlyCollection<string> controllerNames)
        {
            var templateEngine = CreateRazorTemplateEngine();

            List<SyntaxTree> controllerSyntaxTrees = new List<SyntaxTree>(new [] { GetDefaultTemplateSyntaxTree(templateEngine)});
            
            foreach (string controllerName in controllerNames)
            {
                if (_externalTemplateProvider != null)
                {
                    using (Stream stream = _externalTemplateProvider(controllerName))
                    {
                        if (stream != null)
                        {
                            controllerSyntaxTrees.Add(GetSyntaxTreeFromStream(templateEngine, stream, string.Concat(controllerName,"Template")));
                        }
                    }
                }
            }

            Assembly assembly = CompileAssembly(controllerSyntaxTrees);


            return null;
        }

        private Assembly CompileAssembly(IReadOnlyCollection<SyntaxTree> syntaxTrees)
        {
            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Hashtable).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location), // this file (that contains the MyTemplate base class)
                MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "netstandard.dll"))
            };

            var compilation = CSharpCompilation.Create(_outputAssemblyName,
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);
                    StringBuilder messageBuilder = new StringBuilder();

                    foreach (Diagnostic diagnostic in failures)
                    {
                        messageBuilder.AppendFormat("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    throw new TemplateCompilationException(messageBuilder.ToString());
                }


            }
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
                typeof(IApplicationBuilderExtensions).Assembly.GetManifestResourceStream(
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
