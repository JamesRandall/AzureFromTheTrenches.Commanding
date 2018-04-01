using System;
using System.Security.Claims;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    class CommandClaimsBinderProvider : ICommandClaimsBinderProvider
    {
        public bool TryGet(Type modelType, out Action<object, ClaimsPrincipal> binder)
        {
            binder = null;
            return false;
        }
    }
}
