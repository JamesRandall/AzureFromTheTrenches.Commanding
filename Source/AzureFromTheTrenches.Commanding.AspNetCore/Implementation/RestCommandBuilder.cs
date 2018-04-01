using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    internal class RestCommandBuilder : IRestCommandBuilder
    {
        internal ControllerBuilder ControllerBuilder { get; }

        internal string OutputNamespace { get; set; }

        internal Func<string, Stream> ExternalTemplateProvider { get; set; }

        internal Action<string> ConstructedCodeLogger { get; set; }
        
        internal ClaimsMappingBuilder ClaimsMappingBuilder { get; }

        public RestCommandBuilder()
        {
            ClaimsMappingBuilder = new ClaimsMappingBuilder();
            ControllerBuilder = new ControllerBuilder();
            OutputNamespace = "AzureFromTheTrenches.Commanding.AspNetCore.Controllers";
        }

        public IRestCommandBuilder Controllers(Action<IControllerBuilder> controllerBuilderAction)
        {
            controllerBuilderAction(ControllerBuilder);
            return this;
        }

        public IRestCommandBuilder SetOutputNamespace(string outputNamespace)
        {
            OutputNamespace = outputNamespace;
            return this;
        }

        public IRestCommandBuilder SetExternalTemplateProvider(Func<string, Stream> externalTemplateProvider)
        {
            ExternalTemplateProvider = externalTemplateProvider;
            return this;
        }

        public IRestCommandBuilder SetConstructedCodeLogger(Action<string> logger)
        {
            throw new NotImplementedException();
        }

        public void SetDefaultNamespaceOnControllers()
        {
            ControllerBuilder.SetDefaultNamespace(OutputNamespace);
        }

        public IRestCommandBuilder Controller(string controller, Action<IActionBuilder> actionBuilder)
        {
            ControllerBuilder.Controller(controller, actionBuilder);
            return this;
        }

        public IRestCommandBuilder Claims(Action<IClaimsMappingBuilder> claimsMappingBuilder)
        {
            claimsMappingBuilder(ClaimsMappingBuilder);
            return this;
        }
    }
}
