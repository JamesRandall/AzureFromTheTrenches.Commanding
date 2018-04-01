using System;
using System.IO;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    /// <summary>
    /// RestCommandBuilder block for the convention system, uses a fluent API approach
    /// </summary>
    public interface IRestCommandBuilder
    {
        /// <summary>
        /// Define controllers using this builder
        /// </summary>
        IRestCommandBuilder Controllers(Action<IControllerBuilder> controllerBuilderAction);

        /// <summary>
        /// Optional. The output namespace. Defaults to AzureFromTheTrenches.Commanding.AspNetCore.Controllers
        /// </summary>
        IRestCommandBuilder SetOutputNamespace(string outputNamespace);

        /// <summary>
        /// Optional. If set then external templates can be loaded on a per controller basis. The function
        /// will be passed a controller name (from the configuration set on the controller builder) and
        /// should return null if the default template is to be used or a stream to an alternative template.
        /// </summary>
        IRestCommandBuilder SetExternalTemplateProvider(Func<string, Stream> externalTemplateProvider);

        /// <summary>
        /// Optional. If set then as the code for each controller is created it will be passed to this action
        /// allowing visibility of exactly what code is being compiled.
        /// </summary>
        IRestCommandBuilder SetConstructedCodeLogger(Action<string> logger);

        /// <summary>
        /// Define a controller - a short cut for Controllers(cfg => ...)
        /// </summary>
        /// <param name="controller">The name of the controller</param>
        /// <param name="actionBuilder">The action builder</param>
        /// <returns></returns>
        IRestCommandBuilder Controller(string controller, Action<IActionBuilder> actionBuilder);
    }
}
