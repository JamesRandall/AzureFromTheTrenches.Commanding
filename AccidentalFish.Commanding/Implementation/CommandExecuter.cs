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

        public async Task ExecuteAsync<T>(T command) where T : class
        {
            IReadOnlyCollection<PrioritisedCommandActor> actors = _commandRegistry.GetPrioritisedCommandActors<T>();

            foreach (PrioritisedCommandActor actorTemplate in actors)
            {
                object baseActor = _dependencyResolver.Resolve(actorTemplate.CommandActorType);
                ICommandActor<T> actor = baseActor as ICommandActor<T>;
                if (actor != null)
                {
                    await actor.ExecuteAsync(command);
                }
                else
                {
                    ICommandChainActor<T> chainActor = baseActor as ICommandChainActor<T>;
                    if (chainActor != null)
                    {
                        bool shouldContinue = await chainActor.ExecuteAsync(command);
                        if (!shouldContinue)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
