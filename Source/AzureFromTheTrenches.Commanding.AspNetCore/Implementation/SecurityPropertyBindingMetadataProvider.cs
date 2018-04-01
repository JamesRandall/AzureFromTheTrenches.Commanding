using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    class SecurityPropertyBindingMetadataProvider : IBindingMetadataProvider, IDisplayMetadataProvider
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

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context.Key.MetadataKind == ModelMetadataKind.Property)
            {
                // BindingBehavior can fall back to attributes on the Container Type, but we should ignore
                // attributes on the Property Type.
                var bindingBehavior = context.PropertyAttributes?.OfType<SecurityPropertyAttribute>().FirstOrDefault();
                if (bindingBehavior != null)
                {
                    context.DisplayMetadata.ShowForDisplay = false;
                    context.DisplayMetadata.ShowForEdit = false;
                }
            }
        }
    }
}
