using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureStorageAuditing.Commands;

namespace AzureStorageAuditing.Handlers
{
    class ChainCommandHandler : ICommandHandler<ChainCommand, NoResult>
    {
        private readonly ICommandDispatcher _dispatcher;

        public ChainCommandHandler(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public Task<NoResult> ExecuteAsync(ChainCommand command, NoResult previousResult)
        {
            System.Console.WriteLine("About to chain command, correlation IDs should match");
            _dispatcher.DispatchAsync(new OutputToConsoleCommand() { Message = "I've been called from another handler"});
            System.Console.WriteLine("Command chaining complete");
            return Task.FromResult<NoResult>(null);
        }
    }
}
