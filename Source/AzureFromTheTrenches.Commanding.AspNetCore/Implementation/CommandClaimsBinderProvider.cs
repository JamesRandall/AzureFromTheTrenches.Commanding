using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    class CommandClaimsBinderProvider : ICommandClaimsBinderProvider
    {
        private readonly Dictionary<Type, Action<object, ClaimsPrincipal>> _compiledClaimMappers;

        public CommandClaimsBinderProvider(Dictionary<Type, Action<object, ClaimsPrincipal>> compiledClaimMappers)
        {
            _compiledClaimMappers = compiledClaimMappers;
        }

        public bool TryGet(Type modelType, out Action<object, ClaimsPrincipal> binder)
        {
            return _compiledClaimMappers.TryGetValue(modelType, out binder);            
        }
    }
}
