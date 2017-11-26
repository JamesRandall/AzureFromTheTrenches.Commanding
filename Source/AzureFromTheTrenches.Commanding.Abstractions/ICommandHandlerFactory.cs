using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandHandlerFactory
    {
        object Create(Type type);
    }
}
