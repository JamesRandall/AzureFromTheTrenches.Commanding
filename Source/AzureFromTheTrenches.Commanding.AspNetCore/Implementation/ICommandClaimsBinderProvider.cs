using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    internal interface ICommandClaimsBinderProvider
    {
        bool TryGet(Type modelType, out Action<object, ClaimsPrincipal> binder);
    }
}
