using AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle
{
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        /// Adding this to Swagger will hide properties marked with the SecurityPropertyAttribute from
        /// API documentation
        /// </summary>
        /// <param name="options">Swagger options</param>
        /// <returns>The Swagger options for Fluent style usage</returns>
        public static SwaggerGenOptions AddAspNetCoreCommanding(this SwaggerGenOptions options)
        {
            options.SchemaFilter<SecurityPropertySchemaFilter>();
            options.OperationFilter<SecurityPropertyOperationFilter>();
            return options;
        }
    }
}
