using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Compilation.HandlebarsHelpers
{
    internal static class BindingAttributeHelper
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("bindingAttribute", (writer, context, parameters) =>
            {
                if (!(context is ActionDefinition action))
                {
                    throw new TemplateCompilationException("The bindingAttribute helper can only be uesd with an ActionDefinition");
                }

                Type bindingAttributeType = action.BindingAttributeType ?? (
                                                action.Verb == HttpMethod.Get || action.Verb == HttpMethod.Delete
                                                    ? typeof(FromRouteAttribute)
                                                    : typeof(FromBodyAttribute));
                if (!bindingAttributeType.Name.EndsWith("Attribute"))
                {
                    throw new TemplateCompilationException("BindingAttributeType must be an attribute");
                }
                writer.WriteLine($"[{bindingAttributeType.Name.Substring(0, bindingAttributeType.Name.Length - 9)}]");
            });
        }
    }
}
