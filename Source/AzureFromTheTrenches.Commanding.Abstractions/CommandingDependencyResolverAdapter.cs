using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// An adapter that enables the commanding framework to be used with any IoC container
    /// </summary>
    public class CommandingDependencyResolverAdapter : ICommandingDependencyResolverAdapter
    {
        private readonly Action<Type, object> _registerInstance;
        private readonly Action<Type, Type> _typeMapping;
        private readonly Func<Type, object> _resolve;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="registerInstance">A lambda that registers a singleton / instance</param>
        /// <param name="typeMapping">A lambda that registers a transient type</param>
        /// <param name="resolve">A lambda that resolves the given type</param>
        public CommandingDependencyResolverAdapter(Action<Type, object> registerInstance,
            Action<Type, Type> typeMapping,
            Func<Type, object> resolve)
        {
            _registerInstance = registerInstance;
            _typeMapping = typeMapping;
            _resolve = resolve;
        }

        /// <summary>
        /// Registers a singleton / instances
        /// </summary>
        /// <typeparam name="TType">The type of the singleton</typeparam>
        /// <param name="instance">The instance</param>
        /// <returns>The adapter for use in a fluent API</returns>
        public ICommandingDependencyResolverAdapter RegisterInstance<TType>(TType instance)
        {
            _registerInstance(typeof(TType), instance);
            return this;
        }

        /// <summary>
        /// Registers a transient type mappinig
        /// </summary>
        /// <typeparam name="TType">The resolvable type</typeparam>
        /// <typeparam name="TImpl">The implementation type</typeparam>
        /// <returns>The adapter for use in a fluent API</returns>
        public ICommandingDependencyResolverAdapter TypeMapping<TType, TImpl>() where TImpl : TType
        {
            _typeMapping(typeof(TType), typeof(TImpl));
            return this;
        }

        /// <summary>
        /// Registers a transient type mapping
        /// </summary>
        /// <param name="type">The resolvable type</param>
        /// <param name="impl">The implementation type</param>
        /// <returns>The adapter for use in a fluent API</returns>
        public ICommandingDependencyResolverAdapter TypeMapping(Type type, Type impl)
        {
            _typeMapping(type, impl);
            return this;
        }

        /// <summary>
        /// Resolves a type
        /// </summary>
        /// <typeparam name="TType">The type to resolve</typeparam>
        /// <returns>An implementation for the type</returns>
        public TType Resolve<TType>()
        {
            return (TType)_resolve(typeof(TType));
        }

        /// <summary>
        /// Resolves a type
        /// </summary>
        /// <param name="type">The type to resolve</param>
        /// <returns>An implementation for the type</returns>
        public object Resolve(Type type)
        {
            return _resolve(type);
        }

        /// <summary>
        /// The commanding runtime associated with this adapter - this will be set by AddCommanding in the core framework
        /// </summary>
        public ICommandingRuntime AssociatedCommandingRuntime { get; set; }
    }
}