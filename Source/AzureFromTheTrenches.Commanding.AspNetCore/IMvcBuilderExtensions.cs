using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AspNetCore.AspNetInfrastructure;
using AzureFromTheTrenches.Commanding.AspNetCore.Builders;
using AzureFromTheTrenches.Commanding.AspNetCore.Compilation;
using AzureFromTheTrenches.Commanding.AspNetCore.Json;
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
            restCommandBuilderInstance.SetDefaults();

            // When the user has completed defining their command based API through the builder we compile controllers, using Roslyn, using the definitions
            // that have been supplied
            ISyntaxTreeCompiler syntaxTreeCompiler = new SyntaxTreeCompiler(restCommandBuilderInstance.TemplateCompilationReferences);
            IControllerTemplateCompiler controllerTemplateCompiler = new HandlebarsControllerTemplateCompiler(restCommandBuilderInstance.ExternalTemplateProvider);
            IControllerCompiler controllerCompiler = new ControllerCompiler(
                controllerTemplateCompiler,
                syntaxTreeCompiler,
                restCommandBuilderInstance.ConstructedCodeLogger,
                true);
            Assembly assembly = controllerCompiler.Compile(
                restCommandBuilderInstance.ControllerBuilder.Controllers.Values.ToArray(),
                restCommandBuilderInstance.OutputNamespace);
            mvcBuilder.AddApplicationPart(assembly);

            // We pre-compile claim to command mappers for each kind of command in use - these are used by the model binders below
            ICommandClaimsBinderProvider commandClaimsBinderProvider = restCommandBuilderInstance.ClaimsMappingBuilder.Build(restCommandBuilderInstance.GetRegisteredCommandTypes());
            IReadOnlyCollection<MemberInfo> blacklistedMembers = restCommandBuilderInstance.GetSecurityPropertyMembers();

            // Here we add model binders that are able to map claims onto commands. These deliberately can write to the SecurityProperty marked attribute
            // of commands as these are very likely to come from claims. Binders are registered for body, path (URI), and query strings.
            // These wrap the appropriate underlying binder so that the actual model binding takes place using the existing built in binders.
            mvcBuilder
                .AddMvcOptions(options =>
                {
                    IModelBinderProvider bodyModelBinderProvider =
                        options.ModelBinderProviders.Single(x => x is BodyModelBinderProvider);
                    IModelBinderProvider complexTypeModelBinderProvider =
                        options.ModelBinderProviders.Single(x => x is ComplexTypeModelBinderProvider);
                    options.ModelBinderProviders.Insert(0,
                        new ClaimsMappingModelBinderProvider(complexTypeModelBinderProvider,
                            commandClaimsBinderProvider, BindingSource.Query));
                    options.ModelBinderProviders.Insert(0,
                        new ClaimsMappingModelBinderProvider(complexTypeModelBinderProvider,
                            commandClaimsBinderProvider, BindingSource.Path));
                    options.ModelBinderProviders.Insert(0,
                        new ClaimsMappingModelBinderProvider(bodyModelBinderProvider, commandClaimsBinderProvider,
                            BindingSource.Body));
                    options.ModelMetadataDetailsProviders.Add(new SecurityPropertyBindingMetadataProvider());
                })
                .AddJsonOptions(options =>
                {
                    // Because the binding model metadata is ignored on bodyies we have to customize the
                    // serializer and instruct it to ignore the SecurityProperty marked attributes
                    options.SerializerSettings.ContractResolver = new JsonSecurityPropertyContractResolver();
                });

            return mvcBuilder;
        }
    }
}
