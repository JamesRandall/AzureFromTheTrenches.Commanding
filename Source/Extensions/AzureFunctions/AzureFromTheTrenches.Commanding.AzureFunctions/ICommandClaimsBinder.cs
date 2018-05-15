using System.Security.Claims;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public interface ICommandClaimsBinder
    {
        bool Bind(ClaimsPrincipal principal, ICommand command);        
    }
}
