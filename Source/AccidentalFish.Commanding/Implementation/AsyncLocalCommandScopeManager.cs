using System.Threading;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding.Implementation
{
    internal class AsyncLocalCommandScopeManager : ICommandScopeManager
    {
        private readonly ICommandCorrelationIdProvider _commandCorrelationIdProvider;
        private readonly ICommandDispatchContextEnrichment _commandDispatchContextEnrichment;
        private static readonly AsyncLocal<CommandDispatchContext> AsyncLocalCommandContext = new AsyncLocal<CommandDispatchContext>();

        public AsyncLocalCommandScopeManager(ICommandCorrelationIdProvider commandCorrelationIdProvider,
            ICommandDispatchContextEnrichment commandDispatchContextEnrichment)
        {
            _commandCorrelationIdProvider = commandCorrelationIdProvider;
            _commandDispatchContextEnrichment = commandDispatchContextEnrichment;
        }

        public ICommandDispatchContext Enter()
        {
            // NOTE: this only deals with the common case (so far exclusive case) of a command dispatch sequence being
            // initiated with a single command. If multiple commands needed to be simulataneously this would need work
            if (AsyncLocalCommandContext.Value == null)
            {
                // this starts us off with a depth of 0
                AsyncLocalCommandContext.Value = new CommandDispatchContext(_commandCorrelationIdProvider.Create(), _commandDispatchContextEnrichment.GetAdditionalProperties());
            }
            else
            {
                AsyncLocalCommandContext.Value.Increment();
            }
            return AsyncLocalCommandContext.Value;
        }

        public void Exit()
        {
            // the last end called should result in a count of -1 as we start at 0
            if (AsyncLocalCommandContext.Value.Decrement() < 0)
            {
                // NOTE: this only deals with the common case (so far exclusive case) of a command dispatch sequence being
                // initiated with a single command. If multiple commands needed to be simulataneously this would need work
                AsyncLocalCommandContext.Value = null;
            }
        }

        public ICommandDispatchContext GetCurrent()
        {
            return AsyncLocalCommandContext.Value;
        }
    }
}
