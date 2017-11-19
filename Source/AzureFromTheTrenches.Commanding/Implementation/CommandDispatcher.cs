using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
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

        public async Task<CommandResult> DispatchAsync(ICommand command)
        {
            CommandResult<NoResult> result = await DispatchAsync(new NoResultCommandWrapper(command));
            return result;
        }
        
        public async Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command)
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

                    if (!auditRootCommandOnly || dispatchContext.Depth == 0)
                    {
                        await _auditor.Audit(command, Guid.NewGuid(), dispatchContext);
                    }
                }

                try
                {
                    Func<ICommandDispatcher> dispatcherFunc = _commandRegistry.GetCommandDispatcherFactory(command);
                    if (dispatcherFunc != null)
                    {
                        dispatcher = dispatcherFunc();
                        dispatchResult = await dispatcher.DispatchAsync(command);
                        executer = dispatcher.AssociatedExecuter;
                    }

                    if (dispatchResult != null && dispatchResult.DeferExecution)
                    {
                        return new CommandResult<TResult>(default(TResult), true);
                    }
                }
                catch (Exception ex)
                {
                    throw new CommandDispatchException(command, dispatchContext.Copy(), dispatcher?.GetType() ?? GetType(), "Error occurred during command dispatch", ex);
                }
                
                if (executer == null)
                {
                    executer = AssociatedExecuter;
                }
                return new CommandResult<TResult>(await executer.ExecuteAsync(command), false);
            }
            finally
            {
                _commandScopeManager.Exit();
            }
        }

        public ICommandExecuter AssociatedExecuter { get; }
    }
}
