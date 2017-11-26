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
        private readonly ICommandActorExecuter _commandActorExecuter;
        private readonly ICommandActorChainExecuter _commandActorChainExecuter;

        public CommandExecuter(ICommandRegistry commandRegistry,
            ICommandHandlerFactory commandHandlerFactory,
            ICommandScopeManager commandScopeManager,
            ICommandActorExecuter commandActorExecuter,
            ICommandActorChainExecuter commandActorChainExecuter)
        {
            _commandRegistry = commandRegistry;
            _commandHandlerFactory = commandHandlerFactory;
            _commandScopeManager = commandScopeManager;
            _commandActorExecuter = commandActorExecuter;
            _commandActorChainExecuter = commandActorChainExecuter;
        }

        public async Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command)
        {
            IReadOnlyCollection<IPrioritisedCommandHandler> actors = _commandRegistry.GetPrioritisedCommandHandlers(command);
            if (actors == null || actors.Count == 0) throw new MissingCommandActorRegistrationException(command.GetType(), "No command actors registered for execution of command");
            TResult result = default(TResult);

            int actorIndex = 0;
            foreach (IPrioritisedCommandHandler actorTemplate in actors)
            {
                try
                {
                    object baseActor = _commandHandlerFactory.Create(actorTemplate.CommandHandlerType);
                    
                    if (baseActor is ICommandHandler actor)
                    {
                        result = await _commandActorExecuter.ExecuteAsync(actor, command, result);
                    }
                    else
                    {
                        if (baseActor is ICommandChainHandler chainActor)
                        {
                            CommandChainHandlerResult<TResult> chainResult = await _commandActorChainExecuter.ExecuteAsync(chainActor, command, result);
                            result = chainResult.Result;
                            if (chainResult.ShouldStop)
                            {
                                break;
                            }
                        }
                        else
                        {
                            throw new UnableToExecuteActorException("Unexpected result type");                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    ICommandDispatchContext dispatchContext = _commandScopeManager.GetCurrent();
                    throw new CommandExecutionException(command, actorTemplate.CommandHandlerType, actorIndex, dispatchContext?.Copy(), "Error occurred during command execution", ex);
                }
                actorIndex++;
            }

            return result;
        }        
    }
}
