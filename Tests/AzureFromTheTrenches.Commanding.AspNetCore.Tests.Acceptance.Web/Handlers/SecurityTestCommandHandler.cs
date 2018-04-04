using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Handlers
{
    public class SecurityTestCommandHandler : ICommandHandler<SecurityTestCommand>
    {
        public Task ExecuteAsync(SecurityTestCommand command)
        {
            // doesn't need to do anything, just here for an acceptance test
            return Task.CompletedTask;
        }
    }
}
