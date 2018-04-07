using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
        IRestCommandBuilder OutputNamespace(string outputNamespace);

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
        IRestCommandBuilder LogControllerCode(Action<string> logger);

        /// <summary>
        /// Optional. If you are using custom controller templates you will need to provide assemblies for
        /// any references / assemblies you make use of.
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        IRestCommandBuilder TemplateCompilationRefences(IReadOnlyCollection<Assembly> assemblies);

        /// <summary>
        /// Define a controller - a short cut for Controllers(cfg => ...)
        /// </summary>
        /// <param name="controller">The name of the controller</param>
        /// <param name="actionBuilder">The action builder</param>
        /// <returns></returns>
        IRestCommandBuilder Controller(string controller, Action<IActionBuilder> actionBuilder);

        /// <summary>
        /// Allow claims to be mapped to command properties
        /// </summary>
        /// <param name="claimsMappingBuilder">An action that is given a claims mapping builder</param>
        /// <returns></returns>
        IRestCommandBuilder Claims(Action<IClaimsMappingBuilder> claimsMappingBuilder);

        /// <summary>
        /// Set the default route for controllers. Defaults to "api/[controller]"
        /// </summary>
        /// <param name="defaultRoute">The route to use for controllers</param>
        /// <returns></returns>
        IRestCommandBuilder DefaultControllerRoute(string defaultRoute);
    }
}
