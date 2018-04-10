using System;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    /// <summary>
    /// Interface that allows controllers to be configured
    /// </summary>
    public interface IControllerBuilder
    {
        /// <summary>
        /// Configure a controller with the given name and set of actions
        /// </summary>
        /// <param name="controller">The name of the controller</param>
        /// <param name="actionBuilder">Builder for the actions</param>
        /// <returns>The controller builder to enable a fluent API</returns>
        IControllerBuilder Controller(string controller, Action<IActionBuilder> actionBuilder);

        /// <summary>
        /// Configure a controller with the given name, set of actions, and apply filter attributes
        /// </summary>
        /// <param name="controller">The name of the controller</param>
        /// <param name="attributeBuilder">The filter attributes to apply to the controller</param>
        /// <param name="actionBuilder">Builder for the actions</param>
        /// <returns>The controller builder to enable a fluent API</returns>
        IControllerBuilder Controller(string controller,
            Action<IAttributeBuilder> attributeBuilder,
            Action<IActionBuilder> actionBuilder);

        /// <summary>
        /// Configure a controller with the given name, set of actions, and apply filter attributes
        /// </summary>
        /// <param name="controller">The name of the controller</param>
        /// <param name="route">The route to assign to the controller - will attach the attribute [Route("...")]</param>
        /// <param name="attributeBuilder">The filter attributes to apply to the controller</param>
        /// <param name="actionBuilder">Builder for the actions</param>
        /// <returns>The controller builder to enable a fluent API</returns>
        IControllerBuilder Controller(string controller,
            string route,
            Action<IAttributeBuilder> attributeBuilder,
            Action<IActionBuilder> actionBuilder);

        /// <summary>
        /// Configure a controller with the given name, set of actions, and apply filter attributes
        /// </summary>
        /// <param name="controller">The name of the controller</param>
        /// <param name="route">The route to assign to the controller - will attach the attribute [Route("...")]</param>
        /// <param name="actionBuilder">Builder for the actions</param>
        /// <returns>The controller builder to enable a fluent API</returns>
        IControllerBuilder Controller(string controller,
            string route,
            Action<IActionBuilder> actionBuilder);
    }
}
