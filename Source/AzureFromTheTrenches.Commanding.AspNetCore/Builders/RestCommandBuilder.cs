using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Builders
{
    internal class RestCommandBuilder : IRestCommandBuilder
    {
        public RestCommandBuilder()
        {
            ClaimsMappingBuilder = new ClaimsMappingBuilder();
            ControllerBuilder = new ControllerBuilder();
            OutputNamespace = "AzureFromTheTrenches.Commanding.AspNetCore.Controllers";
            _defaultControllerRoute = "api/[controller]";
        }

        IRestCommandBuilder IRestCommandBuilder.Controllers(Action<IControllerBuilder> controllerBuilderAction)
        {
            controllerBuilderAction(ControllerBuilder);
            return this;
        }

        IRestCommandBuilder IRestCommandBuilder.OutputNamespace(string outputNamespace)
        {
            OutputNamespace = outputNamespace;
            return this;
        }

        IRestCommandBuilder IRestCommandBuilder.SetExternalTemplateProvider(Func<string, Stream> externalTemplateProvider)
        {
            ExternalTemplateProvider = externalTemplateProvider;
            return this;
        }

        IRestCommandBuilder IRestCommandBuilder.LogControllerCode(Action<string> logger)
        {
            ConstructedCodeLogger = logger;
            return this;
        }

        IRestCommandBuilder IRestCommandBuilder.TemplateCompilationReferences(params Assembly[] assemblies)
        {
            TemplateCompilationReferences = assemblies;
            return this;
        }

        IRestCommandBuilder IRestCommandBuilder.Controller(string controller, Action<IActionBuilder> actionBuilder)
        {
            ((IControllerBuilder)ControllerBuilder).Controller(controller, actionBuilder);
            return this;
        }

        public IRestCommandBuilder Controller(string controller, Action<IAttributeBuilder> attributeBuilder, Action<IActionBuilder> actionBuilder)
        {
            ((IControllerBuilder) ControllerBuilder).Controller(controller, attributeBuilder, actionBuilder);
            return this;
        }

        public IRestCommandBuilder Controller(string controller, string route, Action<IAttributeBuilder> attributeBuilder, Action<IActionBuilder> actionBuilder)
        {
            ((IControllerBuilder)ControllerBuilder).Controller(controller, route, attributeBuilder, actionBuilder);
            return this;
        }

        IRestCommandBuilder IRestCommandBuilder.Claims(Action<IClaimsMappingBuilder> claimsMappingBuilder)
        {
            claimsMappingBuilder(ClaimsMappingBuilder);
            return this;
        }

        IRestCommandBuilder IRestCommandBuilder.DefaultControllerRoute(string defaultRoute)
        {
            _defaultControllerRoute = defaultRoute;
            return this;
        }

        public void SetDefaults()
        {
            ControllerBuilder.SetDefaults(OutputNamespace, _defaultControllerRoute);
        }

        private string _defaultControllerRoute;

        public ControllerBuilder ControllerBuilder { get; }

        public string OutputNamespace { get; set; }

        public Func<string, Stream> ExternalTemplateProvider { get; private set; }

        public Action<string> ConstructedCodeLogger { get; set; }

        public ClaimsMappingBuilder ClaimsMappingBuilder { get; }

        public IReadOnlyCollection<Assembly> TemplateCompilationReferences { get; private set; }

        public IReadOnlyCollection<Type> GetRegisteredCommandTypes()
        {
            return ControllerBuilder.GetRegisteredCommandTypes();
        }

        public IReadOnlyCollection<MemberInfo> GetSecurityPropertyMembers()
        {
            return new MemberInfo[0];
        }
    }
}
