using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;
using HandlebarsDotNet;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Compilation.HandlebarsHelpers
{
    internal static class AttributesHelper
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("attribute", (writer, context, parameters) =>
            {
                if (!(context is AttributeDefinition attribute))
                {
                    throw new TemplateCompilationException("The attribute helper can only be used with an AttributeDefinition");
                }
                writer.Write("[");
                string attributeName = attribute.AttributeType.FullName.EndsWith("Attribute")
                    ? attribute.AttributeType.FullName.Substring(0, attribute.AttributeType.FullName.Length - "Attribute".Length)
                    : attribute.AttributeType.FullName;
                writer.Write(attributeName);
                if (attribute.HasParameters)
                {
                    writer.Write("(");
                    bool comma = false;
                    foreach (object value in attribute.UnnamedParameters)
                    {
                        if (comma)
                        {
                            writer.Write(",");
                        }
                        writer.Write(WriteParameter(writer, value));
                        comma = true;
                    }

                    foreach (KeyValuePair<string, object> kvp in attribute.NamedParameters)
                    {
                        if (comma)
                        {
                            writer.Write(",");
                        }
                        writer.Write(kvp.Key);
                        writer.Write(": ");
                        writer.Write(WriteParameter(writer, kvp.Value));
                        comma = true;
                    }
                    writer.Write(")");
                }
                writer.Write("]\n");
            });
        }

        private static string WriteParameter(TextWriter writer, object value)
        {
            // we probably need to look more closely at escaping
            if (value is string)
            {
                return $"\"{value.ToString().Replace("\"", "\\\"")}\"";
            }

            return value.ToString();
        }
    }
}
