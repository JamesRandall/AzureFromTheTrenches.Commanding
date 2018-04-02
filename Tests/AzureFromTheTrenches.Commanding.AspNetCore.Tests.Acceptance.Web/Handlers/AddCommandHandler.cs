using System.Linq;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Handlers
{
    public class AddCommandHandler : ICommandHandler<AddCommand, int>
    {
        public Task<int> ExecuteAsync(AddCommand command, int previousResult)
        {
            return Task.FromResult(command.Numbers.Sum());
        }
    }
}
