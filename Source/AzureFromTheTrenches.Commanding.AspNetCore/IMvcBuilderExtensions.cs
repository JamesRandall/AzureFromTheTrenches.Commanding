using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    // ReSharper disable once InconsistentNaming
    public static class IMvcBuilderExtensions
    {
        public static IMvcBuilder AddAspNetCoreCommanding(this IMvcBuilder mvcBuilder,
            Action<IControllerBuilder> builder,
            string outputNamespaceName = "AzureFromTheTrenches.Commanding.AspNetCore.Controllers",
            Func<string, Stream> externalTemplateProvider = null)
        {
            ControllerBuilder builderInstance = new ControllerBuilder(outputNamespaceName);
            builder(builderInstance);

            ISyntaxTreeCompiler syntaxTreeCompiler = new SyntaxTreeCompiler();
            /*IControllerTemplateCompiler controllerTemplateCompiler = new ControllerTemplateCompiler(
                "AzureFromTheTrenches.Commanding.AspNetCore.Controllers.Templates",
                externalTemplateProvider,
                syntaxTreeCompiler);*/
            IControllerTemplateCompiler controllerTemplateCompiler = new HandlebarsControllerTemplateCompiler(externalTemplateProvider);
            
            IControllerCompiler controllerCompiler = new ControllerCompiler(
                controllerTemplateCompiler,
                syntaxTreeCompiler);
            Assembly assembly = controllerCompiler.Compile(builderInstance.Controllers.Values.ToArray(), outputNamespaceName);
            
            mvcBuilder.AddApplicationPart(assembly);
            return mvcBuilder;
        }
    }
}
