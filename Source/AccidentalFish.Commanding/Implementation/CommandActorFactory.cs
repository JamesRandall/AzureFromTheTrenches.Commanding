using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
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
