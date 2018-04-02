using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public interface IActionBuilder
    {
        IActionBuilder Action<TCommand>(HttpMethod method, string route = null)
            where TCommand : ICommand;

        IActionBuilder Action<TCommand, TBindingAttribute>(HttpMethod method, string route = null)
            where TCommand : ICommand
            where TBindingAttribute : Attribute;        

        IActionBuilder Action(ActionDefinition actionDefinition);
    }
}
