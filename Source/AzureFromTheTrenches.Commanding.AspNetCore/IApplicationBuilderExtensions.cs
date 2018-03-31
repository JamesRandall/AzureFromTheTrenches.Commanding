using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AzureFromTheTrenches.Commanding.AspNetCore.Implementation;
using AzureFromTheTrenches.Commanding.AspNetCore.Templates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    // ReSharper disable once InconsistentNaming
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Will configure command routing by building and compiling controllers
        /// </summary>
        /// <param name="appBuilder">The app builder</param>
        /// <param name="builder">Use this hook to define the controllers and their commands</param>
        /// <param name="templateProvider">(Optional)If specified will be given a controller name and can return a Razor template for that controller. Should return null if the built-in template should be used.</param>
        /// <returns></returns>
        public static IApplicationBuilder ConfigureCommandRouting(this IApplicationBuilder appBuilder,
            Action<IControllerBuilder> builder,
            Func<Stream, Assembly> assemblyLoader)
        {
            ControllerBuilder builderInstance = new ControllerBuilder();
            builder(builderInstance);

            IControllerCompiler controllerCompiler = (IControllerCompiler)appBuilder.ApplicationServices.GetService(typeof(IControllerCompiler));
            controllerCompiler.Compile(builderInstance.Controllers.Values.ToArray());

            
            /*using (Stream stream =
                typeof(IApplicationBuilderExtensions).Assembly.GetManifestResourceStream("AzureFromTheTrenches.Commanding.AspNetCore.Templates.DefaultController.cstpl"))
            {
                RazorCodeDocument doc = RazorCodeDocument.Create(RazorSourceDocument.ReadFrom(stream, "controller.cstpl"));
                RazorCSharpDocument csharp = templateEngine.GenerateCode(doc);
                var tree = CSharpSyntaxTree.ParseText(csharp.GeneratedCode);
                
                MetadataReference[] references = new MetadataReference[]
                {                    
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Hashtable).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location), // this file (that contains the MyTemplate base class)
                    MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "netstandard.dll"))
                };

                var compilation = CSharpCompilation.Create($"{outputAssemblyName}.dll",
                    syntaxTrees: new[] { tree },
                    references: references,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                StringBuilder message = new StringBuilder();
                using (var ms = new MemoryStream())
                {
                    EmitResult result = compilation.Emit(ms);
                    if (!result.Success)
                    {
                        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error);

                        foreach (Diagnostic diagnostic in failures)
                        {
                            message.AppendFormat("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                        }

                        //return new ReturnValue<MethodInfo>(false, "The following compile errors were encountered: " + message.ToString(), null);
                    }
                    else
                    {

                        ms.Seek(0, SeekOrigin.Begin);

#if NET451
                            Assembly assembly = Assembly.Load(ms.ToArray());
#else
                        //AssemblyLoadContext context = AssemblyLoadContext.Default;
                        //Assembly assembly = context.LoadFromStream(ms);
                        Assembly assembly = assemblyLoader(ms);
#endif
                        Type templateType = assembly.GetType("AzureFromTheTrenches.Commanding.AspNetCore.Templates.Compiled.Template");
                        ControllerTemplate template = (ControllerTemplate)Activator.CreateInstance(templateType);

                        Type mappingFunction = assembly.GetType("Program");
//                        _functionMethod = mappingFunction.GetMethod("CustomFunction");
                        //_resetMethod = mappingFunction.GetMethod("Reset");
                    }
                }
            }*/

            return appBuilder;
        }
    }
}
