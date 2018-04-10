using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public interface IActionBuilder
    {
        IActionBuilder Action<TCommand>(HttpMethod method)
            where TCommand : ICommand;

        IActionBuilder Action<TCommand>(HttpMethod method, Action<IAttributeBuilder> attributeBuilder)
            where TCommand : ICommand;

        IActionBuilder Action<TCommand>(HttpMethod method, string route)
            where TCommand : ICommand;

        IActionBuilder Action<TCommand>(HttpMethod method, string route, Action<IAttributeBuilder> attributeBuilder)
            where TCommand : ICommand;

        IActionBuilder Action<TCommand, TBindingAttribute>(HttpMethod method)
            where TCommand : ICommand
            where TBindingAttribute : Attribute;

        IActionBuilder Action<TCommand, TBindingAttribute>(HttpMethod method, string route)
            where TCommand : ICommand
            where TBindingAttribute : Attribute;

        IActionBuilder Action<TCommand, TBindingAttribute>(HttpMethod method, Action<IAttributeBuilder> attributeBuilder)
            where TCommand : ICommand
            where TBindingAttribute : Attribute;

        IActionBuilder Action<TCommand, TBindingAttribute>(HttpMethod method, string route, Action<IAttributeBuilder> attributeBuilder)
            where TCommand : ICommand
            where TBindingAttribute : Attribute;



        IActionBuilder Action(ActionDefinition actionDefinition);
    }
}
