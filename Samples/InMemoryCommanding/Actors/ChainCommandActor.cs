using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;
using AccidentalFish.Commanding.Model;
using InMemoryCommanding.Commands;

namespace InMemoryCommanding.Actors
{
    class ChainCommandActor : ICommandActor<ChainCommand, NoResult>
    {
        private readonly ICommandDispatcher _dispatcher;

        public ChainCommandActor(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public Task<NoResult> ExecuteAsync(ChainCommand command, NoResult previousResult)
        {
            System.Console.WriteLine("About to chain command, correlation IDs should match");
            _dispatcher.DispatchAsync(new OutputToConsoleCommand() { Message = "I've been called from another actor"});
            System.Console.WriteLine("Command chaining complete");
            return Task.FromResult<NoResult>(null);
        }
    }
}
