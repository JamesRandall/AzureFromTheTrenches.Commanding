using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface IPrioritisedCommandActor
    {
        int Priority { get; }

        Type CommandActorType { get; }
    }
}
