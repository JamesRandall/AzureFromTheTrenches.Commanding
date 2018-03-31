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
            IControllerTemplateProvider controllerTemplateProvider = new ControllerTemplateProvider(
                "AzureFromTheTrenches.Commanding.AspNetCore.Controllers.Templates",
                externalTemplateProvider,
                new SyntaxTreeCompiler());
            
            resolver.RegisterInstance(controllerTemplateProvider);
            resolver.TypeMapping<ISyntaxTreeCompiler, SyntaxTreeCompiler>();
            resolver.TypeMapping<IControllerCompiler, ControllerCompiler>();            

            return resolver;
        }
    }
}
