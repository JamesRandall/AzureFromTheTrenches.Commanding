using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandingDependencyResolver
    {
        CommandingDependencyResolver RegisterInstance<TType>(TType instance);
        CommandingDependencyResolver TypeMapping<TType, TImpl>() where TImpl : TType;
        CommandingDependencyResolver TypeMapping(Type type, Type impl);
        TType Resolve<TType>();
        object Resolve(Type type);
    }
}