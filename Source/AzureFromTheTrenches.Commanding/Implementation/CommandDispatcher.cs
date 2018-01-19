using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandRegistry _commandRegistry;
        private readonly ICommandScopeManager _commandScopeManager;
        private readonly ICommandAuditPipeline _auditor;

        public CommandDispatcher(ICommandRegistry commandRegistry,
            ICommandExecuter commandExecuter,
            ICommandScopeManager commandScopeManager,
            ICommandAuditPipeline auditPipeline)
        {
            _commandRegistry = commandRegistry;
            _commandScopeManager = commandScopeManager;
            _auditor = auditPipeline;
            AssociatedExecuter = commandExecuter;
        }

        public async Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken)
        {
            await new SynchronizationContextRemover();

            CommandResult<NoResult> result = await DispatchAsync(new NoResultCommandWrapper(command), cancellationToken);
            return result;
        }
        
        public async Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
        {
            await new SynchronizationContextRemover();

            ICommandDispatchContext dispatchContext = _commandScopeManager.Enter();
            try
            {
                CommandResult<TResult> dispatchResult = null;
                ICommandExecuter executer = null;
                ICommandDispatcher dispatcher = null;

                // we specifically audit BEFORE dispatch as this allows us to capture intent and a replay to
                // occur even if dispatch fails
                // (there is also an audit opportunity after execution completes and I'm considering putting one in
                // on dispatch success)
                await _auditor.AuditPreDispatch(command, dispatchContext, cancellationToken);
                
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    Func<ICommandDispatcher> dispatcherFunc = _commandRegistry.GetCommandDispatcherFactory(command);
                    if (dispatcherFunc != null)
                    {
                        dispatcher = dispatcherFunc();
                        dispatchResult = await dispatcher.DispatchAsync(command, cancellationToken);
                        executer = dispatcher.AssociatedExecuter;
                    }
                    await _auditor.AuditPostDispatch(command, dispatchContext, stopwatch.ElapsedMilliseconds, cancellationToken);

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
                return new CommandResult<TResult>(await executer.ExecuteAsync(command, cancellationToken), false);
            }
            finally
            {
                _commandScopeManager.Exit();
            }
        }

        public ICommandExecuter AssociatedExecuter { get; }
    }
}
