using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    internal class ConfigurationBuilder : IConfigurationBuilder
    {
        internal ControllerBuilder ControllerBuilder { get; }

        internal string OutputNamespace { get; set; }

        internal Func<string, Stream> ExternalTemplateProvider { get; set; }

        internal Action<string> ConstructedCodeLogger { get; set; }

        public ConfigurationBuilder()
        {
            ControllerBuilder = new ControllerBuilder();
            OutputNamespace = "AzureFromTheTrenches.Commanding.AspNetCore.Controllers";
        }

        public IConfigurationBuilder Controllers(Action<IControllerBuilder> controllerBuilderAction)
        {
            controllerBuilderAction(ControllerBuilder);
            return this;
        }

        public IConfigurationBuilder SetOutputNamespace(string outputNamespace)
        {
            OutputNamespace = outputNamespace;
            return this;
        }

        public IConfigurationBuilder SetExternalTemplateProvider(Func<string, Stream> externalTemplateProvider)
        {
            ExternalTemplateProvider = externalTemplateProvider;
            return this;
        }

        public IConfigurationBuilder SetConstructedCodeLogger(Action<string> logger)
        {
            throw new NotImplementedException();
        }

        public void SetDefaultNamespaceOnControllers()
        {
            ControllerBuilder.SetDefaultNamespace(OutputNamespace);
        }

        public IConfigurationBuilder Controller(string controller, Action<IActionBuilder> actionBuilder)
        {
            ControllerBuilder.Controller(controller, actionBuilder);
            return this;
        }
    }
}
