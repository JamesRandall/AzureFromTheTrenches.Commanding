using System;
using System.Collections.Generic;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    internal class ActionBuilder : IActionBuilder
    {
        private readonly List<ActionDefinition> _actions = new List<ActionDefinition>();

        public IActionBuilder Action<TCommand, TResult>(HttpMethod method, string route = null) where TCommand : ICommand<TResult>
        {
            _actions.Add(new ActionDefinition
            {
                Route = route,
                Verb = method,
                CommandType = typeof(TCommand),
                ResultType = typeof(TResult)
            });
            return this;
        }

        public IActionBuilder Action<TCommand, TResult, TBindingAttribute>(HttpMethod method, string route = null) where TCommand : ICommand<TResult> where TBindingAttribute : Attribute
        {
            _actions.Add(new ActionDefinition
            {
                Route = route,
                Verb = method,
                CommandType = typeof(TCommand),
                ResultType = typeof(TResult),
                BindingAttributeType = typeof(TBindingAttribute)
            });
            return this;
        }

        public IActionBuilder Action<TCommand>(HttpMethod method, string route = null) where TCommand : ICommand
        {
            _actions.Add(new ActionDefinition
            {
                Route = route,
                Verb = method,
                CommandType = typeof(TCommand),
                ResultType = null
            });
            return this;
        }

        public IActionBuilder Action(ActionDefinition actionDefinition)
        {
            _actions.Add(actionDefinition);
            return this;
        }

        public IReadOnlyCollection<ActionDefinition> Actions => _actions;
    }
}
