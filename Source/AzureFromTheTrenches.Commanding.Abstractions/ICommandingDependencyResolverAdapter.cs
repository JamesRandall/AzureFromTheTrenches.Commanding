using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// An adapter that enables the commanding framework to be used with any IoC container
    /// </summary>
    public interface ICommandingDependencyResolverAdapter
    {
        /// <summary>
        /// Registers a singleton / instances
        /// </summary>
        /// <typeparam name="TType">The type of the singleton</typeparam>
        /// <param name="instance">The instance</param>
        /// <returns>The adapter for use in a fluent API</returns>
        ICommandingDependencyResolverAdapter RegisterInstance<TType>(TType instance);
        /// <summary>
        /// Registers a transient type mappinig
        /// </summary>
        /// <typeparam name="TType">The resolvable type</typeparam>
        /// <typeparam name="TImpl">The implementation type</typeparam>
        /// <returns>The adapter for use in a fluent API</returns>
        ICommandingDependencyResolverAdapter TypeMapping<TType, TImpl>() where TImpl : TType;
        /// <summary>
        /// Registers a transient type mapping
        /// </summary>
        /// <param name="type">The resolvable type</param>
        /// <param name="impl">The implementation type</param>
        /// <returns>The adapter for use in a fluent API</returns>
        ICommandingDependencyResolverAdapter TypeMapping(Type type, Type impl);
        /// <summary>
        /// Resolves a type
        /// </summary>
        /// <typeparam name="TType">The type to resolve</typeparam>
        /// <returns>An implementation for the type</returns>
        TType Resolve<TType>();
        /// <summary>
        /// Resolves a type
        /// </summary>
        /// <param name="type">The type to resolve</param>
        /// <returns>An implementation for the type</returns>
        object Resolve(Type type);
        /// <summary>
        /// The commanding runtime associated with this adapter - this will be set by AddCommanding in the core framework
        /// </summary>
        ICommandingRuntime AssociatedCommandingRuntime { get; set; }
    }
}
