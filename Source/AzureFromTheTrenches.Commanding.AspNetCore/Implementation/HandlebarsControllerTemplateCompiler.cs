using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using AzureFromTheTrenches.Commanding.AspNetCore.Extensions;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    class HandlebarsControllerTemplateCompiler : IControllerTemplateCompiler
    {
        private readonly Func<string, Stream> _externalTemplateProvider;

        static string EvaluateType(Type type)
        {
            StringBuilder retType = new StringBuilder();

            if (type.IsGenericType)
            {
                string[] parentType = type.FullName.Split('`');
                Type[] arguments = type.GetGenericArguments();

                StringBuilder argList = new StringBuilder();
                foreach (Type t in arguments)
                {
                    string arg = EvaluateType(t);
                    if (argList.Length > 0)
                    {
                        argList.AppendFormat(", {0}", arg);
                    }
                    else
                    {
                        argList.Append(arg);
                    }
                }

                if (argList.Length > 0)
                {
                    retType.AppendFormat("{0}<{1}>", parentType[0], argList.ToString());
                }
            }
            else
            {
                return type.ToString();
            }

            return retType.ToString();
        }

        static HandlebarsControllerTemplateCompiler()
        {
            Handlebars.RegisterHelper("actionAttributes", (writer, context, parameters) =>
            {
                if (!(context is ActionDefinition action))
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
                    string evaluatedType = EvaluateType(action.ResultType);
                    writer.WriteLine($"[ProducesResponseType(typeof({evaluatedType}), 200)]");
                }
            });
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
