using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCoreConfigurationBasedCommandControllers.Filters
{
    /// <summary>
    /// This filter is just here to let us test and demonstrate claims mapping without having to wire
    /// up an authentication provider.
    /// </summary>
    public class ClaimsInjectionFilter : IResourceFilter
    {
        private static readonly Guid UserId = Guid.NewGuid();

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            context.HttpContext.User.AddIdentity(new ClaimsIdentity(new []
                {
                    new Claim("UserId", UserId.ToString())
                })
            );
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            
        }
    }
}
