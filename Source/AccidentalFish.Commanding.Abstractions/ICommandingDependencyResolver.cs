using System;

namespace AccidentalFish.Commanding.Abstractions
{
    public interface ICommandingDependencyResolver
    {
        CommandingDependencyResolver RegisterInstance<TType>(TType instance);
        CommandingDependencyResolver TypeMapping<TType, TImpl>() where TImpl : TType;
        TType Resolve<TType>();
        object Resolve(Type type);
    }
}