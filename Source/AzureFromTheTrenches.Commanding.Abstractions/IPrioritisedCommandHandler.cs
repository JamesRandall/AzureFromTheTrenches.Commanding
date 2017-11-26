using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface IPrioritisedCommandHandler
    {
        int Priority { get; }

        Type CommandHandlerType { get; }
    }
}
