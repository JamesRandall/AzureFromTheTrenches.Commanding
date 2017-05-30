using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding.Implementation
{
    internal class CommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandRegistry _commandRegistry;
        private readonly ICommandScopeManager _commandScopeManager;
        private readonly ICommandDispatcherOptions _options;
        private readonly ICommandAuditPipeline _auditor;

        public CommandDispatcher(ICommandRegistry commandRegistry,
            ICommandExecuter commandExecuter,
            ICommandScopeManager commandScopeManager,
            ICommandAuditPipeline auditPipeline,
            ICommandDispatcherOptions options)
        {
            _commandRegistry = commandRegistry;
            _commandScopeManager = commandScopeManager;
            _options = options;
            _auditor = auditPipeline;
            AssociatedExecuter = commandExecuter;
        }

        public async Task<CommandResult<TResult>> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : class
        {
            ICommandDispatchContext dispatchContext = _commandScopeManager.Enter();
            try
            {
                CommandResult<TResult> dispatchResult = null;
                ICommandExecuter executer = null;
                ICommandDispatcher dispatcher = null;

                if (_auditor != null)
                {
                    bool auditRootCommandOnly = _options.AuditRootCommandOnly.HasValue && _options.AuditRootCommandOnly.Value;

                    if (dispatchContext.Depth == 0 || !auditRootCommandOnly)
                    {
                        await _auditor.Audit(command, dispatchContext);
                    }
                }

                try
                {
                    Func<ICommandDispatcher> dispatcherFunc = _commandRegistry.GetCommandDispatcherFactory<TCommand>();
                    if (dispatcherFunc != null)
                    {
                        dispatcher = dispatcherFunc();
                        dispatchResult = await dispatcher.DispatchAsync<TCommand, TResult>(command);
                        executer = dispatcher.AssociatedExecuter;
                    }

                    if (dispatchResult != null && dispatchResult.DeferExecution)
                    {
                        return new CommandResult<TResult>(default(TResult), true);
                    }
                }
                catch (Exception ex)
                {
                    throw new CommandDispatchException<TCommand>(command, dispatchContext.Copy(), dispatcher?.GetType() ?? GetType(), "Error occurred during command dispatch", ex);
                }
                
                if (executer == null)
                {
                    executer = AssociatedExecuter;
                }
                return new CommandResult<TResult>(await executer.ExecuteAsync<TCommand, TResult>(command), false);
            }
            finally
            {
                _commandScopeManager.Exit();
            }
        }

        public Task<CommandResult<NoResult>> DispatchAsync<TCommand>(TCommand command) where TCommand : class
        {
            return DispatchAsync<TCommand, NoResult>(command);
        }

        public ICommandExecuter AssociatedExecuter { get; }
    }
}
