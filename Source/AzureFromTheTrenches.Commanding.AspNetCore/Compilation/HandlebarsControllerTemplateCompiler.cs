using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AzureFromTheTrenches.Commanding.AspNetCore.Compilation.HandlebarsHelpers;
using HandlebarsDotNet;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Compilation
{
    class HandlebarsControllerTemplateCompiler : IControllerTemplateCompiler
    {
        private readonly Func<string, Stream> _externalTemplateProvider;
        
        static HandlebarsControllerTemplateCompiler()
        {
            ActionAttributesHelper.Register();
            AttributesHelper.Register();
            BindingAttributeHelper.Register();
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
