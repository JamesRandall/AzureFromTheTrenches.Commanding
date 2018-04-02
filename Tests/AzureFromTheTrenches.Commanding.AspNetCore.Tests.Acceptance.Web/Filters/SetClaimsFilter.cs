using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Filters
{
    public class SetClaimsFilter : IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            context.HttpContext.User.AddIdentity(new ClaimsIdentity(new[]
                {
                    new Claim("UserId", Constants.UserId.ToString())
                })
            );
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            
        }
    }
}
