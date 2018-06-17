using System;
using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Retrieve information about command and handler registrations
    /// </summary>
    public interface IRegistrationCatalogue
    {
        /// <summary>
        /// Returns a set of all the registered command handlers
        /// </summary>
        /// <returns>Registrered command handlers</returns>
        IReadOnlyCollection<Type> GetRegisteredHandlers();

        /// <summary>
        /// Returns a set of all the registered command dispatcher factories
        /// </summary>
        /// <returns>A set of dispatcher factories</returns>
        IReadOnlyCollection<Func<ICommandDispatcher>> GetRegisteredDispatcherFactories();

        /// <summary>
        /// Returns a set of all the registered commands
        /// </summary>
        /// <returns>A set of command types</returns>
        IReadOnlyCollection<Type> GetRegisteredCommands();
    }
}
