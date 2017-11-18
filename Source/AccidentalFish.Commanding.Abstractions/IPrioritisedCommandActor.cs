using System;

namespace AccidentalFish.Commanding.Abstractions
{
    public interface IPrioritisedCommandActor
    {
        int Priority { get; }

        Type CommandActorType { get; }
    }
}
