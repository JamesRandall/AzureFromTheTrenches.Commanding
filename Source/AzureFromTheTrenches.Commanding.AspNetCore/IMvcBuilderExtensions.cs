using System;
using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.Implementation;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            Action<IRestCommandBuilder> configurationBuilder)
        {
            RestCommandBuilder restCommandBuilderInstance = new RestCommandBuilder();
            configurationBuilder(restCommandBuilderInstance);
            restCommandBuilderInstance.SetDefaultNamespaceOnControllers();

            ISyntaxTreeCompiler syntaxTreeCompiler = new SyntaxTreeCompiler();
            IControllerTemplateCompiler controllerTemplateCompiler = new HandlebarsControllerTemplateCompiler(restCommandBuilderInstance.ExternalTemplateProvider);
            IControllerCompiler controllerCompiler = new ControllerCompiler(
                controllerTemplateCompiler,
                syntaxTreeCompiler);
            Assembly assembly = controllerCompiler.Compile(restCommandBuilderInstance.ControllerBuilder.Controllers.Values.ToArray(),
                restCommandBuilderInstance.OutputNamespace);
            mvcBuilder.AddApplicationPart(assembly);

            mvcBuilder.AddMvcOptions(options =>
            {
                options.ModelMetadataDetailsProviders.Add(new SecurityPropertyBindingMetadataProvider());
            });

            return mvcBuilder;
        }
    }
}
