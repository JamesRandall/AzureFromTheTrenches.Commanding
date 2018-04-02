using System;
using System.Collections.Generic;
using System.IO;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    internal class RestCommandBuilder : IRestCommandBuilder
    {
        public RestCommandBuilder()
        {
            ClaimsMappingBuilder = new ClaimsMappingBuilder();
            ControllerBuilder = new ControllerBuilder();
            OutputNamespace = "AzureFromTheTrenches.Commanding.AspNetCore.Controllers";
        }

        IRestCommandBuilder IRestCommandBuilder.Controllers(Action<IControllerBuilder> controllerBuilderAction)
        {
            controllerBuilderAction(ControllerBuilder);
            return this;
        }

        IRestCommandBuilder IRestCommandBuilder.SetOutputNamespace(string outputNamespace)
        {
            OutputNamespace = outputNamespace;
            return this;
        }

        IRestCommandBuilder IRestCommandBuilder.SetExternalTemplateProvider(Func<string, Stream> externalTemplateProvider)
        {
            ExternalTemplateProvider = externalTemplateProvider;
            return this;
        }

        IRestCommandBuilder IRestCommandBuilder.SetConstructedCodeLogger(Action<string> logger)
        {
            ConstructedCodeLogger = logger;
            return this;
        }

        IRestCommandBuilder IRestCommandBuilder.Controller(string controller, Action<IActionBuilder> actionBuilder)
        {
            ((IControllerBuilder)ControllerBuilder).Controller(controller, actionBuilder);
            return this;
        }

        IRestCommandBuilder IRestCommandBuilder.Claims(Action<IClaimsMappingBuilder> claimsMappingBuilder)
        {
            claimsMappingBuilder(ClaimsMappingBuilder);
            return this;
        }

        public void SetDefaultNamespaceOnControllers()
        {
            ControllerBuilder.SetDefaultNamespace(OutputNamespace);
        }

        public ControllerBuilder ControllerBuilder { get; }

        public string OutputNamespace { get; set; }

        public Func<string, Stream> ExternalTemplateProvider { get; set; }

        public Action<string> ConstructedCodeLogger { get; set; }

        public ClaimsMappingBuilder ClaimsMappingBuilder { get; }

        public IReadOnlyCollection<Type> GetRegisteredCommandTypes()
        {
            return ControllerBuilder.GetRegisteredCommandTypes();
        }
    }
}
