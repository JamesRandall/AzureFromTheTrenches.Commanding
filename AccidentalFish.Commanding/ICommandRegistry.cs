using System.Collections.Generic;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding
{
    public interface ICommandRegistry
    {
        /// <summary>
        /// Register an actor against a command
        /// </summary>
        /// <typeparam name="T1">The type of the command</typeparam>
        /// <typeparam name="T2">The type of the command actor</typeparam>
        /// <param name="dispatcher">Optional command dispatcher</param>
        /// <param name="order">Execution order of the actor</param>
        void Register<T1, T2>(ICommandDispatcher dispatcher = null, int order = CommandActorOrder.Default) where T1 : class where T2 : ICommandActorBase<T1>;

        /// <summary>
        /// Returns the prioritised set of command actors (first to execute is first in the collection)
        /// </summary>
        /// <typeparam name="T">Type of the command</typeparam>
        /// <returns>Set of command actors</returns>
        IReadOnlyCollection<PrioritisedCommandActor> GetPrioritisedCommandActors<T>() where T : class;

        /// <summary>
        /// Gets the command dispatcher for the command
        /// </summary>
        /// <typeparam name="T">The type of the command</typeparam>
        /// <returns>A command dispatcher if one is registered, null if none is</returns>
        ICommandDispatcher GetCommandDispatcher<T>() where T : class;
    }
}
