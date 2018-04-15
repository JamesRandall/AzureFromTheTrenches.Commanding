using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    [Obsolete("This will be deprecated in a future version, please use ICommandingDependencyResolverAdapter instead")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface ICommandingDependencyResolver
    {
        ICommandingDependencyResolver RegisterInstance<TType>(TType instance);
        ICommandingDependencyResolver TypeMapping<TType, TImpl>() where TImpl : TType;
        ICommandingDependencyResolver TypeMapping(Type type, Type impl);
        TType Resolve<TType>();
        object Resolve(Type type);

        ICommandingRuntime AssociatedCommandingRuntime { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}