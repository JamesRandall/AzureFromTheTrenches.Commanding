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

        public async Task<TResult> ExecuteAsync<T, TResult>(T command) where T : class
        {
            IReadOnlyCollection<PrioritisedCommandActor> actors = _commandRegistry.GetPrioritisedCommandActors<T>();
            TResult result = default(TResult);

            foreach (PrioritisedCommandActor actorTemplate in actors)
            {
                
                object baseActor = _dependencyResolver.Resolve(actorTemplate.CommandActorType);
                ICommandActor<T, TResult> actor = baseActor as ICommandActor<T, TResult>;
                if (actor != null)
                {
                    result = await actor.ExecuteAsync(command, result);
                }
                else
                {
                    ICommandChainActor<T, TResult> chainActor = baseActor as ICommandChainActor<T, TResult>;
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
