using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public interface IActionBuilder
    {
        IActionBuilder Action<TCommand, TResult>(HttpMethod method, string route = null)
            where TCommand : ICommand<TResult>;

        IActionBuilder Action<TCommand, TResult, TBindingAttribute>(HttpMethod method, string route = null)
            where TCommand : ICommand<TResult>
            where TBindingAttribute : Attribute;

        IActionBuilder Action<TCommand>(HttpMethod method, string route = null) where TCommand : ICommand;

        IActionBuilder Action(ActionDefinition actionDefinition);
    }
}
