using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation.AspNetInfrastructure
{
    class SecurityPropertyBindingMetadataProvider : IBindingMetadataProvider
    {
        
        public void CreateBindingMetadata(BindingMetadataProviderContext context)
        {
            if (context.Key.MetadataKind == ModelMetadataKind.Property)
            {
                // BindingBehavior can fall back to attributes on the Container Type, but we should ignore
                // attributes on the Property Type.
                var bindingBehavior = context.PropertyAttributes?.OfType<SecurityPropertyAttribute>().FirstOrDefault();
                if (bindingBehavior != null)
                {
                    context.BindingMetadata.IsBindingAllowed = false;
                    context.BindingMetadata.IsBindingRequired = false;
                }
            }
        }
    }
}
