using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            Type commandHandlerType = typeof(TCommandHandler);
            Type candidateCommandType = GetCandidateCommandType(typeof(TCommandHandler));

            return RegisterHandler(candidateCommandType, commandHandlerType, order, dispatcherFactoryFunc);
        }

        public ICommandRegistry Register(Type commandHandlerType, int order = CommandHandlerOrder.Default, Func<ICommandDispatcher> dispatcherFactoryFunc = null)
        {
            Type commandHandlerBase = typeof(ICommandHandlerBase);
            if (!commandHandlerType.GetInterfaces().Any(x => commandHandlerBase.IsAssignableFrom(x)))
            {
                throw new CommandRegistrationException($"Type {commandHandlerType.Name} must implement an interfaces that derives from {commandHandlerBase.Name}");
            }
            
            Type candidateCommandType = GetCandidateCommandType(commandHandlerType);

            return RegisterHandler(candidateCommandType, commandHandlerType, order, dispatcherFactoryFunc);
        }

        private Type GetCandidateCommandType(Type commandHandlerType)
        {
            Type commandHandlerBase = typeof(ICommandHandlerBase);
            Type genericHandlerInterface = commandHandlerType.GetInterfaces().Single(x => x.IsGenericType && commandHandlerBase.IsAssignableFrom(x));

            Type commandType = typeof(ICommand);
            Type candidateCommandType = genericHandlerInterface.GenericTypeArguments.First();
            if (!commandType.IsAssignableFrom(candidateCommandType))
            {
                throw new CommandRegistrationException($"Type {commandHandlerType.Name} must be a generic type and the first generic type must be the command");
            }

            return candidateCommandType;
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
                var unWrappedCommand = wrappedCommand.Command;
                if (!_sortedHandlers.TryGetValue(unWrappedCommand.GetType(),
                    out IReadOnlyCollection<IPrioritisedCommandHandler> wrappedHandlers))
                {
                    throw new MissingCommandHandlerRegistrationException(unWrappedCommand.GetType(),
                        $"No command handlers registered for commands of type {unWrappedCommand.GetType()}");
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

        public ICommandRegistry Discover(params Assembly[] assemblies)
        {
            Type commandHandlerBase = typeof(ICommandHandlerBase);
            foreach (Assembly assembly in assemblies)
            {
                Type[] handlers = assembly.GetTypes().Where(x => commandHandlerBase.IsAssignableFrom(x)).ToArray();
                foreach (Type handler in handlers)
                {
                    Register(handler);
                }
            }

            return this;
        }
    }
}
