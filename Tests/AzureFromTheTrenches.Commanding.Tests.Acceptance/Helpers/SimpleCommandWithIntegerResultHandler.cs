using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers
{
    public class SimpleCommandWithIntegerResultHandler : ICommandHandler<SimpleCommandWithIntegerResult, int>
    {
        public Task<int> ExecuteAsync(SimpleCommandWithIntegerResult command, int previousResult)
        {
            if (command.OptionalException != null)
            {
                throw command.OptionalException;
            }
            return Task.FromResult(99);
        }
    }
}
