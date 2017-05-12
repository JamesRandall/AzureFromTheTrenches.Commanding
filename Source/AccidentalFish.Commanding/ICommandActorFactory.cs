using System;

namespace AccidentalFish.Commanding
{
    public interface ICommandActorFactory
    {
        object Create(Type type);
    }
}
