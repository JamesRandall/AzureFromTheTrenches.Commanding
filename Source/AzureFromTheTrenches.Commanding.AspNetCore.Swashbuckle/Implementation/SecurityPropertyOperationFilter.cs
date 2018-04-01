using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle.Implementation
{
    class SecurityPropertyOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            ModelMetadata metadata = context.ApiDescription.ParameterDescriptions.FirstOrDefault()?.ModelMetadata;

            var parameterDescriptionsToRemove = context.ApiDescription.ParameterDescriptions
                .Select(x => x.ModelMetadata)
                .OfType<DefaultModelMetadata>()
                .Where(x => x.Attributes?.PropertyAttributes != null && x.Attributes.PropertyAttributes.OfType<SecurityPropertyAttribute>().Any())
                .ToArray();
            foreach (var parameterDescription in parameterDescriptionsToRemove)
            {
                IParameter parameter = operation.Parameters.SingleOrDefault(x => x.Name == parameterDescription.PropertyName);
                if (parameter != null)
                {
                    operation.Parameters.Remove(parameter);
                }
            }
        }
    }
}
