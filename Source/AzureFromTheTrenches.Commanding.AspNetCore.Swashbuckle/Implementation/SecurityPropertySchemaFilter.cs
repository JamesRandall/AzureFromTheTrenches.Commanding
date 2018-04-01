using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.Abstractions;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle.Implementation
{
    internal class SecurityPropertySchemaFilter : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
            {
                return;
            }

            var excludedProperties = context.SystemType.GetProperties().Where(t => t.GetCustomAttribute<SecurityPropertyAttribute>() != null);
            foreach (PropertyInfo excludedProperty in excludedProperties)
            {
                if (context.JsonContract is JsonObjectContract jsonObjectContract)
                {
                    string translatedName = jsonObjectContract.Properties.SingleOrDefault(x => x.UnderlyingName == excludedProperty.Name)?.PropertyName;
                    if (translatedName != null)
                    {
                        if (schema.Properties.ContainsKey(translatedName))
                        {
                            schema.Properties.Remove(translatedName);
                            continue;
                        }
                    }
                }

                if (schema.Properties.ContainsKey(excludedProperty.Name))
                {
                    schema.Properties.Remove(excludedProperty.Name);
                }
            }
        }
    }
}
