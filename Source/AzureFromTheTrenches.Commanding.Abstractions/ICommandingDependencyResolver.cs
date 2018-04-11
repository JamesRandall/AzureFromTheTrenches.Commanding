using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    [Obsolete("This will be deprecated in a future version, please use ICommandingDependencyResolverAdapter instead")]
    public interface ICommandingDependencyResolver
    {
        ICommandingDependencyResolver RegisterInstance<TType>(TType instance);
        ICommandingDependencyResolver TypeMapping<TType, TImpl>() where TImpl : TType;
        ICommandingDependencyResolver TypeMapping(Type type, Type impl);
        TType Resolve<TType>();
        object Resolve(Type type);

        ICommandingRuntime AssociatedCommandingRuntime { get; set; }
    }
}