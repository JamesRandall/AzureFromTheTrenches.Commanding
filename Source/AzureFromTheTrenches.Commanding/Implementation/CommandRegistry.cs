﻿using System;
using System.Collections.Generic;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandRegistry : ICommandRegistry
    {
        private readonly ICommandHandlerExecuter _executer;
        private readonly Action<Type> _commandHandlerContainerRegistration;
        private readonly Dictionary<Type, SortedSet<PrioritisedCommandHandler>> _handlers = new Dictionary<Type, SortedSet<PrioritisedCommandHandler>>();
        private readonly Dictionary<Type, Func<ICommandDispatcher>> _commandDispatchers = new Dictionary<Type, Func<ICommandDispatcher>>();

        public CommandRegistry(ICommandHandlerExecuter executer, Action<Type> commandHandlerContainerRegistration = null)
        {
            _executer = executer;
            _commandHandlerContainerRegistration = commandHandlerContainerRegistration;
        }
        
        public ICommandRegistry Register<TCommandHandler>(int order = CommandHandlerOrder.Default, Func<ICommandDispatcher> dispatcherFactoryFunc = null) where TCommandHandler : ICommandHandlerBase
        {
            Type commandHandlerBase = typeof(ICommandHandlerBase);
            Type handlerType = typeof(TCommandHandler);
            Type genericHandlerInterface = handlerType.GetInterfaces().Single(x => x.IsGenericType && commandHandlerBase.IsAssignableFrom(x));

            Type commandType = typeof(ICommand);
            Type candidateCommandType = genericHandlerInterface.GenericTypeArguments.First();
            if (!commandType.IsAssignableFrom(candidateCommandType))
            {
                throw new CommandRegistrationException($"Type {handlerType.Name} must be a generic type and the first generic type must be the command");
            }

            return RegisterHandler(candidateCommandType, handlerType, order, dispatcherFactoryFunc);
        }

        private ICommandRegistry RegisterHandler(Type commandType, Type commandHandlerType, int order,
            Func<ICommandDispatcher> dispatcherFactoryFunc)
        {
            if (!_handlers.TryGetValue(commandType, out var set))
            {
                set = new SortedSet<PrioritisedCommandHandler>();
                _handlers.Add(commandType, set);
            }

            set.Add(new PrioritisedCommandHandler(order, commandHandlerType));
            if (dispatcherFactoryFunc != null)
            {
                _commandDispatchers[commandType] = dispatcherFactoryFunc;
            }

            // Register the handler with a container if that is needed
            _commandHandlerContainerRegistration?.Invoke(commandHandlerType);

            // Compile an executer
            _executer.CompileHandlerExecuter(commandType, commandHandlerType);

            return this;
        }

        public ICommandRegistry Register<TCommand, TResult>(Func<ICommandDispatcher> dispatcherFactoryFunc) where TCommand : ICommand<TResult>
        {
            _commandDispatchers[typeof(TCommand)] = dispatcherFactoryFunc ?? throw new ArgumentNullException(nameof(dispatcherFactoryFunc));
            return this;
        }

        public IReadOnlyCollection<IPrioritisedCommandHandler> GetPrioritisedCommandHandlers(ICommand command)
        {
            if (!_handlers.TryGetValue(command.GetType(), out var set))
            {
                if (command is NoResultCommandWrapper wrappedCommand)
                {
                    if (!_handlers.TryGetValue(wrappedCommand.Command.GetType(), out set))
                    {
                        return null;
                    }
                }
            }
            return set.ToArray();
        }

        public Func<ICommandDispatcher> GetCommandDispatcherFactory(ICommand command)
        {
            _commandDispatchers.TryGetValue(command.GetType(), out var dispatcherFactoryFunc);
            return dispatcherFactoryFunc;
        }
    }
}
