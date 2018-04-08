using System;
using System.Security.Claims;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation.AspNetInfrastructure
{
    internal interface ICommandClaimsBinderProvider
    {
        bool TryGet(Type modelType, out Action<object, ClaimsPrincipal> binder);
    }
}
