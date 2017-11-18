using System;
using AccidentalFish.Commanding.Abstractions;

namespace AccidentalFish.Commanding.Implementation
{
    internal class CommandActorFactory : ICommandActorFactory
    {
        private readonly Func<Type, object> _creatorFunc;

        public CommandActorFactory(Func<Type, object> creatorFunc)
        {
            _creatorFunc = creatorFunc;
        }

        public object Create(Type type)
        {
            return _creatorFunc(type);
        }
    }
}
