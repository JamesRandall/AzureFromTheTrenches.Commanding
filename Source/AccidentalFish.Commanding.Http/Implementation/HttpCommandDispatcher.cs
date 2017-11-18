using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;

namespace AccidentalFish.Commanding.Http.Implementation
{
    class HttpCommandDispatcher : ICommandDispatcher
    {
        public HttpCommandDispatcher(ICommandExecuter httpCommandExecuter)
        {
            AssociatedExecuter = httpCommandExecuter;
        }

        public Task<CommandResult<TResult>> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : class
        {
            return Task.FromResult(new CommandResult<TResult>(default(TResult), false));
        }

        public Task<CommandResult<NoResult>> DispatchAsync<TCommand>(TCommand command) where TCommand : class
        {
            return DispatchAsync<TCommand, NoResult>(command);
        }

        public Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command)
        {
            throw new System.NotImplementedException();
        }

        public ICommandExecuter AssociatedExecuter { get; }
    }
}
