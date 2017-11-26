using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandHandlerFactory : ICommandHandlerFactory
    {
        private readonly Func<Type, object> _creatorFunc;

        public CommandHandlerFactory(Func<Type, object> creatorFunc)
        {
            _creatorFunc = creatorFunc;
        }

        public object Create(Type type)
        {
            return _creatorFunc(type);
        }
    }
}
