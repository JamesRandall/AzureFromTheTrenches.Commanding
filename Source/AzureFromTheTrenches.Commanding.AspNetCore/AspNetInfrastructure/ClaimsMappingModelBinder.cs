using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AzureFromTheTrenches.Commanding.AspNetCore.AspNetInfrastructure
{
    class ClaimsMappingModelBinder : IModelBinder
    {
        private readonly IModelBinder _decoratedModelBinder;
        private readonly ICommandClaimsBinderProvider _commandClaimsBinderProvider;
        
        public ClaimsMappingModelBinder(IModelBinder decoratedModelBinder,
            ICommandClaimsBinderProvider commandClaimsBinderProvider)
        {
            _decoratedModelBinder = decoratedModelBinder;
            _commandClaimsBinderProvider = commandClaimsBinderProvider;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await _decoratedModelBinder.BindModelAsync(bindingContext);
            if (bindingContext.HttpContext.User != null && _commandClaimsBinderProvider.TryGet(bindingContext.ModelType, out Action<object, ClaimsPrincipal> binder))
            {
                binder(bindingContext.Result.Model, bindingContext.HttpContext.User);
            }
        }
    }
}
