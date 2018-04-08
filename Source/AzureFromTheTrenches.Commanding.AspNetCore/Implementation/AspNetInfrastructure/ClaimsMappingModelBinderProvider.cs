using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation.AspNetInfrastructure
{
    internal class ClaimsMappingModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinderProvider _decoratedModelBinderProvider;
        private readonly ICommandClaimsBinderProvider _commandClaimsBinderProvider;
        private readonly BindingSource _bindingSource;
        private static readonly Type CommandInterfaceType = typeof(ICommand);

        public ClaimsMappingModelBinderProvider(IModelBinderProvider decoratedModelBinderProvider,
            ICommandClaimsBinderProvider commandClaimsBinderProvider,
            BindingSource bindingSource)
        {
            _decoratedModelBinderProvider = decoratedModelBinderProvider;
            _commandClaimsBinderProvider = commandClaimsBinderProvider;
            _bindingSource = bindingSource;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.BindingInfo.BindingSource != null &&
                context.BindingInfo.BindingSource.CanAcceptDataFrom(_bindingSource) &&
                CommandInterfaceType.IsAssignableFrom(context.Metadata.ModelType))
            {
                IModelBinder modelBinder = _decoratedModelBinderProvider.GetBinder(context);
                return new ClaimsMappingModelBinder(modelBinder, _commandClaimsBinderProvider);
            }

            return null;
        }
    }
}
