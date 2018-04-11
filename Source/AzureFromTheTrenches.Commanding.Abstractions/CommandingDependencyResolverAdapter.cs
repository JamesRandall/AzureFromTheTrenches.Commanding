using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public class CommandingDependencyResolverAdapter : ICommandingDependencyResolverAdapter
    {
        private readonly Action<Type, object> _registerInstance;
        private readonly Action<Type, Type> _typeMapping;
        private readonly Func<Type, object> _resolve;

        public CommandingDependencyResolverAdapter(Action<Type, object> registerInstance,
            Action<Type, Type> typeMapping,
            Func<Type, object> resolve)
        {
            _registerInstance = registerInstance;
            _typeMapping = typeMapping;
            _resolve = resolve;
        }

        public ICommandingDependencyResolverAdapter RegisterInstance<TType>(TType instance)
        {
            _registerInstance(typeof(TType), instance);
            return this;
        }

        public ICommandingDependencyResolverAdapter TypeMapping<TType, TImpl>() where TImpl : TType
        {
            _typeMapping(typeof(TType), typeof(TImpl));
            return this;
        }

        public ICommandingDependencyResolverAdapter TypeMapping(Type type, Type impl)
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
}