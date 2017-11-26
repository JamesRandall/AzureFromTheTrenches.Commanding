using System;
using System.Collections.Generic;
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
        private readonly ICommandHandlerChainExecuter _commandHandlerChainExecuter;
        private readonly ICommandExecutionExceptionHandler _commandExecutionExceptionHandler;

        public CommandExecuter(ICommandRegistry commandRegistry,
            ICommandHandlerFactory commandHandlerFactory,
            ICommandScopeManager commandScopeManager,
            ICommandHandlerExecuter commandHandlerExecuter,
            ICommandHandlerChainExecuter commandHandlerChainExecuter,
            ICommandExecutionExceptionHandler commandExecutionExceptionHandler)
        {
            _commandRegistry = commandRegistry;
            _commandHandlerFactory = commandHandlerFactory;
            _commandScopeManager = commandScopeManager;
            _commandHandlerExecuter = commandHandlerExecuter;
            _commandHandlerChainExecuter = commandHandlerChainExecuter;
            _commandExecutionExceptionHandler = commandExecutionExceptionHandler;
        }

        public async Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command)
        {
            IReadOnlyCollection<IPrioritisedCommandHandler> handlers = _commandRegistry.GetPrioritisedCommandHandlers(command);
            if (handlers == null || handlers.Count == 0) throw new MissingCommandHandlerRegistrationException(command.GetType(), "No command actors registered for execution of command");
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
                        result = await _commandHandlerExecuter.ExecuteAsync(handler, command, result);
                    }
                    else
                    {
                        if (baseHandler is ICommandChainHandler chainHandler)
                        {
                            CommandChainHandlerResult<TResult> chainResult = await _commandHandlerChainExecuter.ExecuteAsync(chainHandler, command, result);
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
                    ICommandDispatchContext dispatchContext = _commandScopeManager.GetCurrent();
                    bool shouldContinue = await _commandExecutionExceptionHandler.HandleException(ex, baseHandler, handlerIndex, command, dispatchContext);
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
