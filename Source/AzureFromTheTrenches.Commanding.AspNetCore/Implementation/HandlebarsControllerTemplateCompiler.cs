using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.AspNetCore.Extensions;
using AzureFromTheTrenches.Commanding.AspNetCore.Implementation.Model;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    class HandlebarsControllerTemplateCompiler : IControllerTemplateCompiler
    {
        private readonly Func<string, Stream> _externalTemplateProvider;

        static HandlebarsControllerTemplateCompiler()
        {
            Handlebars.RegisterHelper("actionAttributes", (writer, context, parameters) =>
            {
                if (!(context is AugmentedActionDefinition action))
                {
                    throw new TemplateCompilationException("The actionAttributes helper can only be uesd with an ActionDefinition");
                }
                
                writer.WriteLine(action.Verb.ToControllerAttribute());
                if (!string.IsNullOrWhiteSpace(action.Route))
                {
                    writer.WriteLine($"[Route(\"{action.Route}\")]");
                }

                if (action.ResultType != null)
                {
                    writer.WriteLine($"[ProducesResponseType(typeof({action.ResultType.FullName}), 200)]");
                }
            });
            Handlebars.RegisterHelper("bindingAttribute", (writer, context, parameters) =>
            {
                if (!(context is AugmentedActionDefinition action))
                {
                    throw new TemplateCompilationException("The bindingAttribute helper can only be uesd with an ActionDefinition");
                }

                Type bindingAttributeType = action.BindingAttributeType ?? (
                                                action.Verb == HttpMethod.Get || action.Verb == HttpMethod.Delete
                                                ? typeof(FromQueryAttribute)
                                                : typeof(FromBodyAttribute));
                if (!bindingAttributeType.Name.EndsWith("Attribute"))
                {
                    throw new TemplateCompilationException("BindingAttributeType must be an attribute");
                }
                writer.WriteLine($"[{bindingAttributeType.Name.Substring(0, bindingAttributeType.Name.Length-9)}]");
            });
        }

        public HandlebarsControllerTemplateCompiler(Func<string, Stream> externalTemplateProvider)
        {
            _externalTemplateProvider = externalTemplateProvider;
        }

        public Dictionary<string, Func<object, string>> CompileTemplates(IReadOnlyCollection<string> controllerNames)
        {
            Func<object, string> defaultTemplate = GetDefaultTemplate();
            if (_externalTemplateProvider == null)
            {
                return controllerNames.ToDictionary(x => x, y => defaultTemplate);
            }
            Dictionary<string, Func<object,string>> result = new Dictionary<string, Func<object, string>>();
            foreach (string controllerName in controllerNames)
            {
                Func<object, string> template = GetExternalTemplate(controllerName) ?? defaultTemplate;
                result[controllerName] = template;
            }

            return result;
        }

        private Func<object, string> GetDefaultTemplate()
        {
            using (Stream stream =
                GetType().Assembly.GetManifestResourceStream(
                    "AzureFromTheTrenches.Commanding.AspNetCore.Templates.DefaultController.handlebars"))
            using (StreamReader streamReader = new StreamReader(stream))
            {
                string templateString = streamReader.ReadToEnd();
                Func<object,string> template = Handlebars.Compile(templateString);
                return template;
            }
        }

        private Func<object, string> GetExternalTemplate(string controllerName)
        {
            using (Stream stream = _externalTemplateProvider(controllerName))
            {
                if (stream == null)
                {
                    return null;
                }
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    string templateString = streamReader.ReadToEnd();
                    Func<object, string> template = Handlebars.Compile(templateString);
                    return template;
                }
            }
        }
    }
}
