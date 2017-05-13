using System;
using System.Collections.Generic;
using System.Linq;
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

        public ICommandRegistry Register<TCommand, TCommandActor>(int order = CommandActorOrder.Default, Func<ICommandDispatcher> dispatcherFactoryFunc = null) where TCommand : class where TCommandActor : ICommandActorBase<TCommand>
        {
            SortedSet<PrioritisedCommandActor> set;
            if (!_actors.TryGetValue(typeof(TCommand), out set))
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

        public ICommandRegistry Register<TCommand>(Func<ICommandDispatcher> dispatcherFactoryFunc) where TCommand : class
        {
            _commandDispatchers[typeof(TCommand)] = dispatcherFactoryFunc ?? throw new ArgumentNullException(nameof(dispatcherFactoryFunc));
            return this;
        }

        public IReadOnlyCollection<PrioritisedCommandActor> GetPrioritisedCommandActors<T>() where T : class
        {
            SortedSet<PrioritisedCommandActor> set;
            if (!_actors.TryGetValue(typeof(T), out set))
            {
                return null;
            }
            return set.ToList();
        }

        public Func<ICommandDispatcher> GetCommandDispatcherFactory<T>() where T : class
        {
            Func<ICommandDispatcher> dispatcherFactoryFunc;
            _commandDispatchers.TryGetValue(typeof(T), out dispatcherFactoryFunc);
            return dispatcherFactoryFunc;
        }
    }
}
