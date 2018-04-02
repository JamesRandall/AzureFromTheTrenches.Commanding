using System;
using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.Implementation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
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
            Action<IRestCommandBuilder> configurationBuilder)
        {
            RestCommandBuilder restCommandBuilderInstance = new RestCommandBuilder();
            configurationBuilder(restCommandBuilderInstance);
            restCommandBuilderInstance.SetDefaultNamespaceOnControllers();

            ISyntaxTreeCompiler syntaxTreeCompiler = new SyntaxTreeCompiler();
            IControllerTemplateCompiler controllerTemplateCompiler = new HandlebarsControllerTemplateCompiler(restCommandBuilderInstance.ExternalTemplateProvider);
            IControllerCompiler controllerCompiler = new ControllerCompiler(
                controllerTemplateCompiler,
                syntaxTreeCompiler,
                restCommandBuilderInstance.ClaimsMappingBuilder);
            Assembly assembly = controllerCompiler.Compile(restCommandBuilderInstance.ControllerBuilder.Controllers.Values.ToArray(),
                restCommandBuilderInstance.OutputNamespace);
            mvcBuilder.AddApplicationPart(assembly);

            ICommandClaimsBinderProvider commandClaimsBinderProvider = restCommandBuilderInstance.ClaimsMappingBuilder.Build(restCommandBuilderInstance.GetRegisteredCommandTypes());

            mvcBuilder.AddMvcOptions(options =>
            {
                IModelBinderProvider bodyModelBinderProvider = options.ModelBinderProviders.Single(x => x is BodyModelBinderProvider);
                IModelBinderProvider complexTypeModelBinderProvider = options.ModelBinderProviders.Single(x => x is ComplexTypeModelBinderProvider);
                options.ModelBinderProviders.Insert(0,
                    new ClaimsMappingModelBinderProvider(complexTypeModelBinderProvider, commandClaimsBinderProvider, BindingSource.Query));
                options.ModelBinderProviders.Insert(0,
                    new ClaimsMappingModelBinderProvider(bodyModelBinderProvider, commandClaimsBinderProvider, BindingSource.Body));
                options.ModelMetadataDetailsProviders.Add(new SecurityPropertyBindingMetadataProvider());
            });

            return mvcBuilder;
        }
    }
}
