using System;
using System.IO;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Implementation;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public static class AspNetCoreCommandingDependencies
    {
        public static ICommandingDependencyResolver UseAspNetCoreCommanding(this ICommandingDependencyResolver resolver,            
            Func<string, Stream> externalTemplateProvider = null)
        {
            IControllerTemplateCompiler controllerTemplateCompiler = new ControllerTemplateCompiler(
                "AzureFromTheTrenches.Commanding.AspNetCore.Controllers.Templates",
                externalTemplateProvider,
                new SyntaxTreeCompiler());
            
            resolver.RegisterInstance(controllerTemplateCompiler);
            resolver.TypeMapping<ISyntaxTreeCompiler, SyntaxTreeCompiler>();
            resolver.TypeMapping<IControllerCompiler, ControllerCompiler>();            

            return resolver;
        }
    }
}
