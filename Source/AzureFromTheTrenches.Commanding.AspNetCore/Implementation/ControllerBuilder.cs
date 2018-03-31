using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    internal class ControllerBuilder : IControllerBuilder
    {
        private readonly string _namespaceName;
        private readonly Dictionary<string, ControllerDefinition> _controllers = new Dictionary<string, ControllerDefinition>();

        public ControllerBuilder(string namespaceName)
        {
            _namespaceName = namespaceName;
        }

        public IControllerBuilder Controller(string controller, Action<IActionBuilder> actionBuilder)
        {
            return Controller(controller, null, actionBuilder);
        }

        public IControllerBuilder Controller(string controller, string route, Action<IActionBuilder> actionBuilder) // TODO: we need to allow attributes to be specified
        {
            if (_controllers.ContainsKey(controller))
            {
                throw new ArgumentException(nameof(controller), $"The controller {controller} has already been configured.");
            }

            string resolvedName =
                controller.EndsWith("Controller") ? controller : string.Concat(controller, "Controller");
            ActionBuilder actionBuilderInstance = new ActionBuilder();
            actionBuilder(actionBuilderInstance);
            _controllers[resolvedName] = new ControllerDefinition
            {
                Actions = actionBuilderInstance.Actions,
                Name = resolvedName,
                Namespace = _namespaceName,
                Route = route
            };
            return this;
        }

        public IDictionary<string, ControllerDefinition> Controllers => _controllers;
    }
}
