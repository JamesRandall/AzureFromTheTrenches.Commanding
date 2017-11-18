using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;
using AccidentalFish.Commanding.Model;
using AccidentalFish.DependencyResolver;

namespace AccidentalFish.Commanding.Implementation
{
    internal class CommandExecuter : ICommandExecuter
    {
        private readonly ICommandRegistry _commandRegistry;
        private readonly ICommandActorFactory _commandActorFactory;
        private readonly INoResultCommandActorBaseExecuter _noResultCommandActorBaseExecuter;
        private readonly ICommandScopeManager _commandScopeManager;

        public CommandExecuter(ICommandRegistry commandRegistry,
            ICommandActorFactory commandActorFactory,
            INoResultCommandActorBaseExecuter noResultCommandActorBaseExecuter,
            ICommandScopeManager commandScopeManager)
        {
            _commandRegistry = commandRegistry;
            _commandActorFactory = commandActorFactory;
            _noResultCommandActorBaseExecuter = noResultCommandActorBaseExecuter;
            _commandScopeManager = commandScopeManager;
        }

        public async Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command) where TCommand : class
        {
            IReadOnlyCollection<IPrioritisedCommandActor> actors = _commandRegistry.GetPrioritisedCommandActors<TCommand>();
            if (actors == null || actors.Count == 0) throw new MissingCommandActorRegistrationException(command.GetType(), "No command actors registered for execution of command");
            TResult result = default(TResult);

            int actorIndex = 0;
            foreach (PrioritisedCommandActor actorTemplate in actors)
            {
                try
                {
                    object baseActor = _commandActorFactory.Create(actorTemplate.CommandActorType);
                    ICommandActor<TCommand, TResult> actor = baseActor as ICommandActor<TCommand, TResult>;
                    if (actor != null)
                    {
                        result = await actor.ExecuteAsync(command, result);
                    }
                    else
                    {
                        ICommandChainActor<TCommand, TResult> chainActor = baseActor as ICommandChainActor<TCommand, TResult>;
                        if (chainActor != null)
                        {
                            CommandChainActorResult<TResult> chainResult = await chainActor.ExecuteAsync(command, result);
                            result = chainResult.Result;
                            if (chainResult.ShouldStop)
                            {
                                break;
                            }
                        }
                        else
                        {
                            // this allows commands dispatched with no expectation of a result to be executed
                            // regardless of the actor definitions
                            // it doesn't currently deal with chained commands
                            if (typeof(TResult) == typeof(NoResult) && baseActor is ICommandActorBase<TCommand>)
                            {
                                await _noResultCommandActorBaseExecuter.ExecuteAsync(baseActor, command);
                            }
                            else
                            {
                                throw new UnableToExecuteActorException("Unexpected result type");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ICommandDispatchContext dispatchContext = _commandScopeManager.GetCurrent();
                    throw new CommandExecutionException<TCommand>(command, actorTemplate.CommandActorType, actorIndex, dispatchContext?.Copy(), "Error occurred during command execution", ex);
                }
                actorIndex++;
            }

            return result;
        }
    }
}
