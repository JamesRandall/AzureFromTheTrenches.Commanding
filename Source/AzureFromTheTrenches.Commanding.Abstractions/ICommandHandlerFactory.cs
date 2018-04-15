using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Creates command handlers given their type
    /// </summary>
    public interface ICommandHandlerFactory
    {
        /// <summary>
        /// Creates a command handler of the given type
        /// </summary>
        /// <param name="type">The type of the handler</param>
        /// <returns>A newly created command handler</returns>
        object Create(Type type);
    }
}
