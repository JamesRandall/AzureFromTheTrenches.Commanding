using System.Threading;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding.Implementation
{
    internal class AsyncLocalCommandScopeManager : ICommandScopeManager
    {
        private readonly ICommandCorrelationIdProvider _commandCorrelationIdProvider;
        private readonly ICommandContextEnrichment _commandContextEnrichment;
        private static readonly AsyncLocal<CommandContext> AsyncLocalCommandContext = new AsyncLocal<CommandContext>();

        public AsyncLocalCommandScopeManager(ICommandCorrelationIdProvider commandCorrelationIdProvider,
            ICommandContextEnrichment commandContextEnrichment)
        {
            _commandCorrelationIdProvider = commandCorrelationIdProvider;
            _commandContextEnrichment = commandContextEnrichment;
        }

        public ICommandContext Enter()
        {
            // NOTE: this only deals with the common case (so far exclusive case) of a command dispatch sequence being
            // initiated with a single command. If multiple commands needed to be simulataneously this would need work
            if (AsyncLocalCommandContext.Value == null)
            {
                // this starts us off with a depth of 0
                AsyncLocalCommandContext.Value = new CommandContext(_commandCorrelationIdProvider.Create(), _commandContextEnrichment.GetAdditionalProperties());
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

        public ICommandContext GetCurrent()
        {
            return AsyncLocalCommandContext.Value;
        }
    }
}
