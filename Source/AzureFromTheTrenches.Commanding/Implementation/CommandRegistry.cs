using System;
using System.Collections.Generic;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandRegistry : ICommandRegistry
    {
        private readonly ICommandHandlerExecuter _executer;
        private readonly Action<Type> _commandHandlerContainerRegistration;
        private readonly Dictionary<Type, Func<ICommandDispatcher>> _commandDispatchers = new Dictionary<Type, Func<ICommandDispatcher>>();
        private readonly Dictionary<Type, IReadOnlyCollection<IPrioritisedCommandHandler>> _sortedHandlers = new Dictionary<Type, IReadOnlyCollection<IPrioritisedCommandHandler>>();

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
            if (!_sortedHandlers.TryGetValue(commandType, out var handlers))
            {
                handlers = new PrioritisedCommandHandler[0];
            }

            SortedSet<IPrioritisedCommandHandler> set = new SortedSet<IPrioritisedCommandHandler>(handlers);
            set.Add(new PrioritisedCommandHandler(order, commandHandlerType));
            _sortedHandlers[commandType] = set.ToArray();

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

        public ICommandRegistry Register<TCommand>(Func<ICommandDispatcher> dispatcherFactoryFunc) where TCommand : ICommand
        {
            _commandDispatchers[typeof(TCommand)] = dispatcherFactoryFunc ?? throw new ArgumentNullException(nameof(dispatcherFactoryFunc));
            return this;
        }

        public IReadOnlyCollection<IPrioritisedCommandHandler> GetPrioritisedCommandHandlers(ICommand command)
        {
            if (command is NoResultCommandWrapper wrappedCommand)
            {
                if (!_sortedHandlers.TryGetValue(wrappedCommand.Command.GetType(),
                    out IReadOnlyCollection<IPrioritisedCommandHandler> wrappedHandlers))
                {
                    throw new MissingCommandHandlerRegistrationException(command.GetType(),
                        $"No command handlers registered for commands of type {command.GetType()}");
                }
                return wrappedHandlers;
            }

            if (!_sortedHandlers.TryGetValue(command.GetType(),
                out IReadOnlyCollection<IPrioritisedCommandHandler> result))
            {
                throw new MissingCommandHandlerRegistrationException(command.GetType(),
                    $"No command handlers registered for commands of type {command.GetType()}");
            }
            return result;
        }

        public Func<ICommandDispatcher> GetCommandDispatcherFactory(ICommand command)
        {
            Type commandType = command.GetType();
            if (command is NoResultCommandWrapper wrappedCommand)
            {
                commandType = wrappedCommand.Command.GetType();
            }
            _commandDispatchers.TryGetValue(commandType, out var dispatcherFactoryFunc);
            return dispatcherFactoryFunc;
        }
    }
}
