using System.Threading.Tasks;
using AspNetCoreConfigurationBasedCommandControllers.Commands;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AspNetCoreConfigurationBasedCommandControllers.Handlers
{
    public class GetMessageQueryHandler : ICommandHandler<GetMessageQuery, string>
    {
        public Task<string> ExecuteAsync(GetMessageQuery command, string previousResult)
        {
            return Task.FromResult("Hello World!");
        }
    }
}
