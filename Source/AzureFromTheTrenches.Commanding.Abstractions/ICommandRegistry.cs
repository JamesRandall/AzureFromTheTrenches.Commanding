using System;
using System.Collections.Generic;
using System.Reflection;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandRegistry
    {
        /// <summary>
        /// Register an handler against a command with no expected result
        /// </summary>
        /// <typeparam name="TCommandHandler">The type of the command handler</typeparam>
        /// <param name="dispatcherFactoryFunc">Optional command dispatcher factory function</param>
        /// <param name="order">Execution order of the handler</param>
        ICommandRegistry Register<TCommandHandler>(int order = CommandHandlerOrder.Default, Func<ICommandDispatcher> dispatcherFactoryFunc = null)
            where TCommandHandler : ICommandHandlerBase;

        /// <summary>
        /// Register an handler against a command with no expected result
        /// </summary>
        /// <param name="dispatcherFactoryFunc">Optional command dispatcher factory function</param>
        /// <param name="commandHandlerType">The type of the command handler</param>
        /// <param name="order">Execution order of the handler</param>
        ICommandRegistry Register(Type commandHandlerType, int order = CommandHandlerOrder.Default, Func<ICommandDispatcher> dispatcherFactoryFunc = null);

        /// <summary>
        /// Register a command with a dispatcher but no handler. Typically this is used when the command actors are known to be deferred via a queue
        /// or that are remotely executed
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="dispatcherFactoryFunc">Command dispatcher factory function</param>
        ICommandRegistry Register<TCommand, TResult>(Func<ICommandDispatcher> dispatcherFactoryFunc) where TCommand : ICommand<TResult>;

        /// <summary>
        /// Register a command with a dispatcher but no handler. Typically this is used when the command actors are known to be deferred via a queue
        /// or that are remotely executed
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <param name="dispatcherFactoryFunc">Command dispatcher factory function</param>
        ICommandRegistry Register<TCommand>(Func<ICommandDispatcher> dispatcherFactoryFunc) where TCommand : ICommand;

        /// <summary>
        /// Returns the prioritised set of command actors (first to execute is first in the collection)
        /// </summary>
        /// <typeparam name="T">Type of the command</typeparam>
        /// <returns>Set of command actors</returns>
        IReadOnlyCollection<IPrioritisedCommandHandler> GetPrioritisedCommandHandlers(ICommand command);

        /// <summary>
        /// Gets the command dispatcher for the command
        /// </summary>
        /// <typeparam name="T">The type of the command</typeparam>
        /// <returns>A function able to create a command dispatcher if one is registered, null if none is</returns>
        Func<ICommandDispatcher> GetCommandDispatcherFactory(ICommand command);

        /// <summary>
        /// Will search the specified assembly for command handlers and register them
        /// </summary>
        /// <param name="assemblies">The assemblies to search</param>
        /// <returns>The command registery for use in a fluent call style</returns>
        ICommandRegistry Discover(params Assembly[] assemblies);

        /// <summary>
        /// Will search the assemly that TTypeInAssembly belongs in 
        /// for command handlers and register them
        /// </summary>        
        /// <returns>The command registery for use in a fluent call style</returns>
        ICommandRegistry Discover<TTypeInAssembly>();
    }
}
