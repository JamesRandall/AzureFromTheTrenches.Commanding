using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandingDependencyResolver
    {
        ICommandingDependencyResolver RegisterInstance<TType>(TType instance);
        ICommandingDependencyResolver TypeMapping<TType, TImpl>() where TImpl : TType;
        ICommandingDependencyResolver TypeMapping(Type type, Type impl);
        TType Resolve<TType>();
        object Resolve(Type type);
    }
}