using System;
using System.IO;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Implementation;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public static class AspNetCoreCommandingDependencies
    {
        public static ICommandingDependencyResolver UseAspNetCoreCommanding(this ICommandingDependencyResolver resolver,            
            string outputNamespaceName = "AzureFromTheTrenches.Commanding.AspNetCore.Controllers",
            Func<string, Stream> externalTemplateProvider = null)
        {
            IRazorTemplateProvider razorTemplateProvider = new RazorTemplateProvider(outputNamespaceName, externalTemplateProvider);
            
            resolver.RegisterInstance(razorTemplateProvider);
            resolver.TypeMapping<IControllerCompiler, ControllerCompiler>();

            return resolver;
        }
    }
}
