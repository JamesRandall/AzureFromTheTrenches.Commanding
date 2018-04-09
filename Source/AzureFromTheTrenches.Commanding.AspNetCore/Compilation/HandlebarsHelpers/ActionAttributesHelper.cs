using System;
using System.Text;
using AzureFromTheTrenches.Commanding.AspNetCore.Extensions;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;
using HandlebarsDotNet;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Compilation.HandlebarsHelpers
{
    internal static class ActionAttributesHelper
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("actionAttributes", (writer, context, parameters) =>
            {
                if (!(context is ActionDefinition action))
                {
                    throw new TemplateCompilationException("The actionAttributes helper can only be uesd with an ActionDefinition");
                }

                writer.WriteLine(action.Verb.ToControllerAttribute());
                if (!String.IsNullOrWhiteSpace(action.Route))
                {
                    writer.WriteLine($"[Route(\"{action.Route}\")]");
                }

                if (action.ResultType != null)
                {
                    string evaluatedType = Utils.EvaluateType(action.ResultType);
                    writer.WriteLine($"[ProducesResponseType(typeof({evaluatedType}), 200)]");
                }
            });
        }
    }
}
