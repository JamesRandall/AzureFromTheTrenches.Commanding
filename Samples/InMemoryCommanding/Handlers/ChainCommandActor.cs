using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using InMemoryCommanding.Commands;

namespace InMemoryCommanding.Handlers
{
    class NestingCommandHandler : ICommandHandler<NestingCommand, NoResult>
    {
        private readonly ICommandDispatcher _dispatcher;

        public NestingCommandHandler(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public Task<NoResult> ExecuteAsync(NestingCommand command, NoResult previousResult)
        {
            System.Console.WriteLine("About to nest command calls, correlation IDs should match");
            _dispatcher.DispatchAsync(new OutputToConsoleCommand() { Message = "I've been called from another handler"});
            System.Console.WriteLine("Command nesting complete");
            return Task.FromResult<NoResult>(null);
        }
    }
}
