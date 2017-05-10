using System.Threading.Tasks;

namespace AccidentalFish.Commanding.Implementation
{
    internal class CommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandRegistry _commandRegistry;
        private readonly ICommandExecuter _commandExecuter;

        public CommandDispatcher(ICommandRegistry commandRegistry, ICommandExecuter commandExecuter)
        {
            _commandRegistry = commandRegistry;
            _commandExecuter = commandExecuter;
        }

        public async Task<bool> DispatchAsync<T>(T command) where T : class
        {
            bool shouldExecuteImmediately = true;
            ICommandDispatcher dispatcher = _commandRegistry.GetCommandDispatcher<T>();
            if (dispatcher != null)
            {
                shouldExecuteImmediately = await dispatcher.DispatchAsync(command);
            }

            if (shouldExecuteImmediately)
            {
                await _commandExecuter.ExecuteAsync(command);
            }

            return shouldExecuteImmediately;
        }
    }
}
