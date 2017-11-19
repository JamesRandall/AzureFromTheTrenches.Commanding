using System;

namespace AccidentalFish.Commanding.Abstractions
{
    public class CommandingDependencyResolver
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

        public CommandingDependencyResolver RegisterInstance<TType>(TType instance)
        {
            _registerInstance(typeof(TType), instance);
            return this;
        }

        public CommandingDependencyResolver TypeMapping<TType, TImpl>() where TImpl : TType
        {
            _typeMapping(typeof(TType), typeof(TImpl));
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
    }
}
