using System;
using System.Collections.Generic;

namespace AccidentalFish.Commanding.Abstractions
{
    public interface ICommandRegistry
    {
        /// <summary>
        /// Register an actor against a command with an expected result
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <typeparam name="TCommandActor">The type of the command actor</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="dispatcherFactoryFunc">Optional command dispatcher factory function</param>
        /// <param name="order">Execution order of the actor</param>
        ICommandRegistry Register<TCommand, TResult, TCommandActor>(int order = CommandActorOrder.Default, Func<ICommandDispatcher> dispatcherFactoryFunc = null)
            where TCommand : ICommand<TResult> where TCommandActor : ICommandActor<TCommand, TResult>;

        /// <summary>
        /// Register an actor against a command with no expected result
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <typeparam name="TCommandActor">The type of the command actor</typeparam>
        /// <param name="dispatcherFactoryFunc">Optional command dispatcher factory function</param>
        /// <param name="order">Execution order of the actor</param>
        ICommandRegistry Register<TCommand, TCommandActor>(int order = CommandActorOrder.Default, Func<ICommandDispatcher> dispatcherFactoryFunc = null)
            where TCommand : ICommand where TCommandActor : ICommandActor<TCommand>;

        /// <summary>
        /// Register a command with a dispatcher but no actor. Typically this is used when the command actors are deferred via a queue or that are remotely executed
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="dispatcherFactoryFunc">Command dispatcher factory function</param>
        ICommandRegistry Register<TCommand, TResult>(Func<ICommandDispatcher> dispatcherFactoryFunc) where TCommand : ICommand<TResult>;

        /// <summary>
        /// Returns the prioritised set of command actors (first to execute is first in the collection)
        /// </summary>
        /// <typeparam name="T">Type of the command</typeparam>
        /// <returns>Set of command actors</returns>
        IReadOnlyCollection<IPrioritisedCommandActor> GetPrioritisedCommandActors(ICommand command);

        /// <summary>
        /// Gets the command dispatcher for the command
        /// </summary>
        /// <typeparam name="T">The type of the command</typeparam>
        /// <returns>A function able to create a command dispatcher if one is registered, null if none is</returns>
        Func<ICommandDispatcher> GetCommandDispatcherFactory(ICommand command);
    }
}
