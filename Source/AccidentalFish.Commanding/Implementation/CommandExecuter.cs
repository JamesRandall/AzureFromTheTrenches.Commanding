using System.Collections.Generic;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Model;
using AccidentalFish.DependencyResolver;

namespace AccidentalFish.Commanding.Implementation
{
    internal class CommandExecuter : ICommandExecuter
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly ICommandRegistry _commandRegistry;

        public CommandExecuter(IDependencyResolver dependencyResolver, ICommandRegistry commandRegistry)
        {
            _dependencyResolver = dependencyResolver;
            _commandRegistry = commandRegistry;
        }

        public async Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command) where TCommand : class
        {
            IReadOnlyCollection<PrioritisedCommandActor> actors = _commandRegistry.GetPrioritisedCommandActors<TCommand>();
            if (actors == null || actors.Count == 0) throw new MissingCommandActorRegistrationException(command.GetType(), "No command actors registered for execution of command");
            TResult result = default(TResult);

            foreach (PrioritisedCommandActor actorTemplate in actors)
            {
                
                object baseActor = _dependencyResolver.Resolve(actorTemplate.CommandActorType);
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
                }
            }

            return result;
        }
    }
}
