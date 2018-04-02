using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandingDependencyResolverAdapter
    {
        ICommandingDependencyResolverAdapter RegisterInstance<TType>(TType instance);
        ICommandingDependencyResolverAdapter TypeMapping<TType, TImpl>() where TImpl : TType;
        ICommandingDependencyResolverAdapter TypeMapping(Type type, Type impl);
        TType Resolve<TType>();
        object Resolve(Type type);
    }
}
