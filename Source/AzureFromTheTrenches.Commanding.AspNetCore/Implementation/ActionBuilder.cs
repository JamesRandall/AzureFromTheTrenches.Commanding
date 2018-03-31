using System.Collections.Generic;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    internal class ActionBuilder : IActionBuilder
    {
        private readonly List<ActionDefinition> _actions = new List<ActionDefinition>();

        public IActionBuilder Action<TCommand, TResult>(HttpMethod method, string action = null)
        {
            _actions.Add(new ActionDefinition
            {
                OptionalActionName = action,
                Verb = method
            });
            return this;
        }

        public IReadOnlyCollection<ActionDefinition> Actions => _actions;
    }
}
