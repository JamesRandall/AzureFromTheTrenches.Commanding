using System;
using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    // ReSharper disable once InconsistentNaming
    public static class IMvcBuilderExtensions
    {
        /// <summary>
        /// Adds configuration based REST commanding to ASP.Net Core
        /// </summary>
        /// <param name="mvcBuilder">The MVC Builder extended</param>
        /// <param name="configurationBuilder">An action accepting a configuration builder</param>
        /// <returns>The MVC builder</returns>
        public static IMvcBuilder AddAspNetCoreCommanding(this IMvcBuilder mvcBuilder,
            Action<IConfigurationBuilder> configurationBuilder)
        {
            ConfigurationBuilder configurationBuilderInstance = new ConfigurationBuilder();
            configurationBuilder(configurationBuilderInstance);
            configurationBuilderInstance.SetDefaultNamespaceOnControllers();

            ISyntaxTreeCompiler syntaxTreeCompiler = new SyntaxTreeCompiler();
            IControllerTemplateCompiler controllerTemplateCompiler = new HandlebarsControllerTemplateCompiler(configurationBuilderInstance.ExternalTemplateProvider);
            IControllerCompiler controllerCompiler = new ControllerCompiler(
                controllerTemplateCompiler,
                syntaxTreeCompiler);
            Assembly assembly = controllerCompiler.Compile(configurationBuilderInstance.ControllerBuilder.Controllers.Values.ToArray(),
                configurationBuilderInstance.OutputNamespace);
            mvcBuilder.AddApplicationPart(assembly);

            return mvcBuilder;
        }
    }
}
