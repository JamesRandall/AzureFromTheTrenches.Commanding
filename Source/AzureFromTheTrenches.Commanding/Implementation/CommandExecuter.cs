using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandExecuter : ICommandExecuter
    {
        private readonly ICommandRegistry _commandRegistry;
        private readonly ICommandHandlerFactory _commandHandlerFactory;
        private readonly ICommandScopeManager _commandScopeManager;
        private readonly ICommandHandlerExecuter _commandHandlerExecuter;
        private readonly IPipelineAwareCommandHandlerExecuter _pipelineAwareCommandHandlerExecuter;
        private readonly ICommandExecutionExceptionHandler _commandExecutionExceptionHandler;
        private readonly ICommandAuditPipeline _commandAuditPipeline;
        
        public CommandExecuter(ICommandRegistry commandRegistry,
            ICommandHandlerFactory commandHandlerFactory,
            ICommandScopeManager commandScopeManager,
            ICommandHandlerExecuter commandHandlerExecuter,
            IPipelineAwareCommandHandlerExecuter pipelineAwareCommandHandlerExecuter,
            ICommandExecutionExceptionHandler commandExecutionExceptionHandler,
            ICommandAuditPipeline commandAuditPipeline)
        {
            _commandRegistry = commandRegistry;
            _commandHandlerFactory = commandHandlerFactory;
            _commandScopeManager = commandScopeManager;
            _commandHandlerExecuter = commandHandlerExecuter;
            _pipelineAwareCommandHandlerExecuter = pipelineAwareCommandHandlerExecuter;
            _commandExecutionExceptionHandler = commandExecutionExceptionHandler;
            _commandAuditPipeline = commandAuditPipeline;
        }

        public async Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
        {
            await new SynchronizationContextRemover();

            ICommandDispatchContext dispatchContext = _commandScopeManager.GetCurrent();
            
            try
            {
                TResult result = await ExecuteCommandWithHandlers(command, dispatchContext, cancellationToken);
                await _commandAuditPipeline.AuditExecution(command, dispatchContext, true, cancellationToken);
                return result;
            }
            catch (Exception)
            {
                await _commandAuditPipeline.AuditExecution(command, dispatchContext, false, cancellationToken);
                throw;
            }
        }

        private async Task<TResult> ExecuteCommandWithHandlers<TResult>(ICommand<TResult> command, ICommandDispatchContext dispatchContext, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<IPrioritisedCommandHandler> handlers = _commandRegistry.GetPrioritisedCommandHandlers(command);
            if (handlers == null || handlers.Count == 0)
                throw new MissingCommandHandlerRegistrationException(command.GetType(),
                    "No command actors registered for execution of command");
            TResult result = default(TResult);

            int handlerIndex = 0;
            foreach (IPrioritisedCommandHandler handlerTemplate in handlers)
            {
                object baseHandler = null;
                try
                {
                    baseHandler = _commandHandlerFactory.Create(handlerTemplate.CommandHandlerType);

                    if (baseHandler is ICommandHandler handler)
                    {
                        result = await _commandHandlerExecuter.ExecuteAsync(handler, command, result, cancellationToken);
                    }
                    else
                    {
                        if (baseHandler is IPipelineAwareCommandHandler chainHandler)
                        {
                            PipelineAwareCommandHandlerResult<TResult> chainResult =
                                await _pipelineAwareCommandHandlerExecuter.ExecuteAsync(chainHandler, command, result, cancellationToken);
                            result = chainResult.Result;
                            if (chainResult.ShouldStop)
                            {
                                break;
                            }
                        }
                        else
                        {
                            throw new UnableToExecuteHandlerException("Unexpected result type");
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                    bool shouldContinue =
                        await _commandExecutionExceptionHandler.HandleException(ex, baseHandler, handlerIndex, command,
                            dispatchContext);
                    if (!shouldContinue)
                    {
                        break;
                    }
                }
                handlerIndex++;
            }

            return result;
        }
    }
}
