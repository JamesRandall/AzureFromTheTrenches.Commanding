using System.Collections.Generic;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding
{
    public interface ICommandRegistry
    {
        /// <summary>
        /// Register an actor against a command
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <typeparam name="TCommandActor">The type of the command actor</typeparam>
        /// <param name="dispatcher">Optional command dispatcher</param>
        /// <param name="order">Execution order of the actor</param>
        void Register<TCommand, TCommandActor>(int order = CommandActorOrder.Default, ICommandDispatcher dispatcher = null) where TCommand : class where TCommandActor : ICommandActorBase<TCommand>;

        /// <summary>
        /// Register a command with a dispatcher but no actor. Typically this is used when the command actors are deferred via a queue or that are remotely executed
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="dispatcher"></param>
        void Register<TCommand>(ICommandDispatcher dispatcher) where TCommand : class;

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
