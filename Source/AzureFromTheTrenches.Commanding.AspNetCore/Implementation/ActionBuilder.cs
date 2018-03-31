using System.Collections.Generic;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    internal class ActionBuilder : IActionBuilder
    {
        private readonly List<ActionDefinition> _actions = new List<ActionDefinition>();

        public IActionBuilder Action<TCommand, TResult>(HttpMethod method, string action = null) where TCommand : ICommand<TResult>
        {
            _actions.Add(new ActionDefinition
            {
                OptionalActionName = action,
                Verb = method,
                CommandType = typeof(TCommand),
                ResultType = typeof(TResult)
            });
            return this;
        }

        public IActionBuilder Action<TCommand>(HttpMethod method, string action = null) where TCommand : ICommand
        {
            _actions.Add(new ActionDefinition
            {
                OptionalActionName = action,
                Verb = method,
                CommandType = typeof(TCommand),
                ResultType = null
            });
            return this;
        }

        public IReadOnlyCollection<ActionDefinition> Actions => _actions;
    }
}
