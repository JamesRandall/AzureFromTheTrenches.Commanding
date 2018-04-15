using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    [Obsolete("This will be deprecated in a future version, please use CommandingDependencyResolverAdapter instead")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CommandingDependencyResolver : ICommandingDependencyResolver
    {
        private readonly Action<Type, object> _registerInstance;
        private readonly Action<Type, Type> _typeMapping;
        private readonly Func<Type, object> _resolve;

        public CommandingDependencyResolver(Action<Type, object> registerInstance,
            Action<Type, Type> typeMapping,
            Func<Type, object> resolve)
        {
            _registerInstance = registerInstance;
            _typeMapping = typeMapping;
            _resolve = resolve;
        }

        public ICommandingDependencyResolver RegisterInstance<TType>(TType instance)
        {
            _registerInstance(typeof(TType), instance);
            return this;
        }

        public ICommandingDependencyResolver TypeMapping<TType, TImpl>() where TImpl : TType
        {
            _typeMapping(typeof(TType), typeof(TImpl));
            return this;
        }

        public ICommandingDependencyResolver TypeMapping(Type type, Type impl) 
        {
            _typeMapping(type, impl);
            return this;
        }

        public TType Resolve<TType>()
        {
            return (TType)_resolve(typeof(TType));
        }

        public object Resolve(Type type)
        {
            return _resolve(type);
        }

        public ICommandingRuntime AssociatedCommandingRuntime { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
