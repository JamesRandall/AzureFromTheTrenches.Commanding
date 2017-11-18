using System;

namespace AccidentalFish.Commanding.Abstractions
{
    public interface ICommandActorFactory
    {
        object Create(Type type);
    }
}
