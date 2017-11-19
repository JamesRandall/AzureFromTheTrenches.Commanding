using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandActorFactory
    {
        object Create(Type type);
    }
}
