using System;
using System.Collections.Generic;
using System.Linq;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding.Implementation
{
    internal class CommandRegistry : ICommandRegistry
    {
        private readonly Action<Type> _commandActorContainerRegistration;
        private readonly Dictionary<Type, SortedSet<PrioritisedCommandActor>> _actors = new Dictionary<Type, SortedSet<PrioritisedCommandActor>>();
        private readonly Dictionary<Type, Func<ICommandDispatcher>> _commandDispatchers = new Dictionary<Type, Func<ICommandDispatcher>>();

        public CommandRegistry(Action<Type> commandActorContainerRegistration = null)
        {
            _commandActorContainerRegistration = commandActorContainerRegistration;
        }
        
        public ICommandRegistry Register<TCommand, TResult, TCommandActor>(int order = CommandActorOrder.Default,
            Func<ICommandDispatcher> dispatcherFactoryFunc = null) where TCommand : ICommand<TResult> where TCommandActor : ICommandActor<TCommand, TResult>
        {
            return RegisterActor<TCommand, TCommandActor>(order, dispatcherFactoryFunc);
        }

        public ICommandRegistry Register<TCommand, TCommandActor>(int order = CommandActorOrder.Default,
            Func<ICommandDispatcher> dispatcherFactoryFunc = null) where TCommand : ICommand where TCommandActor : ICommandActor<TCommand>
        {
            return RegisterActor<TCommand, TCommandActor>(order, dispatcherFactoryFunc);
        }

        private ICommandRegistry RegisterActor<TCommand, TCommandActor>(int order, Func<ICommandDispatcher> dispatcherFactoryFunc)
            where TCommand : ICommand where TCommandActor : ICommandActor
        {
            if (!_actors.TryGetValue(typeof(TCommand), out var set))
            {
                set = new SortedSet<PrioritisedCommandActor>();
                _actors.Add(typeof(TCommand), set);
            }

            set.Add(new PrioritisedCommandActor(order, typeof(TCommandActor)));
            if (dispatcherFactoryFunc != null)
            {
                _commandDispatchers[typeof(TCommand)] = dispatcherFactoryFunc;
            }

            // Register the actor with a container if that is needed
            _commandActorContainerRegistration?.Invoke(typeof(TCommandActor));

            return this;
        }

        public ICommandRegistry Register<TCommand, TResult>(Func<ICommandDispatcher> dispatcherFactoryFunc) where TCommand : ICommand<TResult>
        {
            _commandDispatchers[typeof(TCommand)] = dispatcherFactoryFunc ?? throw new ArgumentNullException(nameof(dispatcherFactoryFunc));
            return this;
        }

        public IReadOnlyCollection<IPrioritisedCommandActor> GetPrioritisedCommandActors(ICommand command)
        {
            if (!_actors.TryGetValue(command.GetType(), out var set))
            {
                if (command is NoResultCommandWrapper wrappedCommand)
                {
                    if (!_actors.TryGetValue(wrappedCommand.Command.GetType(), out set))
                    {
                        return null;
                    }
                }
            }
            return set.ToList();
        }

        public Func<ICommandDispatcher> GetCommandDispatcherFactory(ICommand command)
        {
            _commandDispatchers.TryGetValue(command.GetType(), out var dispatcherFactoryFunc);
            return dispatcherFactoryFunc;
        }
    }
}
