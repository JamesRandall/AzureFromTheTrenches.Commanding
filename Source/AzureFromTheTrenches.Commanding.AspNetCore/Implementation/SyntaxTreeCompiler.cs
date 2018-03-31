using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    internal class SyntaxTreeCompiler : ISyntaxTreeCompiler
    {
        public Assembly CompileAssembly(string outputAssemblyName, IReadOnlyCollection<SyntaxTree> syntaxTrees)
        {
            // TODO: We need to allow consumers of the package to be able to add their own metadata references / assemblies
            // so that they can add filter attributes in packages we haven't directly references (even custom fitlers)
            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(Abstractions.ICommand).GetTypeInfo().Assembly.Location), 
                MetadataReference.CreateFromFile(typeof(Microsoft.AspNetCore.Mvc.Controller).GetTypeInfo().Assembly.Location), // microsoft.aspnetcore.mvc
                MetadataReference.CreateFromFile(typeof(Microsoft.AspNetCore.Mvc.ControllerBase).GetTypeInfo().Assembly.Location), // microsoft.aspnetcore.mvc.core
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Hashtable).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location), // this file (that contains the MyTemplate base class)
                MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "netstandard.dll"))
            };

            var compilation = CSharpCompilation.Create(outputAssemblyName,
                syntaxTrees,
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

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

                ms.Seek(0, SeekOrigin.Begin);
                AssemblyLoadContext context = AssemblyLoadContext.Default;
                Assembly assembly = context.LoadFromStream(ms);
                return assembly;
            }
        }
    }
}
