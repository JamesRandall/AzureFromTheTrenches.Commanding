using System;
using System.Collections.Generic;
using System.Linq;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding.Implementation
{
    internal class CommandRegistry : ICommandRegistry
    {
        private readonly Dictionary<Type, SortedSet<PrioritisedCommandActor>> _actors = new Dictionary<Type, SortedSet<PrioritisedCommandActor>>();
        private readonly Dictionary<Type, ICommandDispatcher> _commandDispatchers = new Dictionary<Type, ICommandDispatcher>();

        public void Register<TCommand, TCommandActor>(int order = CommandActorOrder.Default, ICommandDispatcher dispatcher = null) where TCommand : class where TCommandActor : ICommandActorBase<TCommand>
        {
            SortedSet<PrioritisedCommandActor> set;
            if (!_actors.TryGetValue(typeof(TCommand), out set))
            {
                set = new SortedSet<PrioritisedCommandActor>();
                _actors.Add(typeof(TCommand), set);
            }

            set.Add(new PrioritisedCommandActor(order, typeof(TCommandActor)));
            if (dispatcher != null)
            {
                _commandDispatchers[typeof(TCommand)] = dispatcher;
            }
        }

        public void Register<TCommand>(ICommandDispatcher dispatcher) where TCommand : class
        {
            _commandDispatchers[typeof(TCommand)] = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));            
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

        public ICommandDispatcher GetCommandDispatcher<T>() where T : class
        {
            ICommandDispatcher dispatcher;
            _commandDispatchers.TryGetValue(typeof(T), out dispatcher);
            return dispatcher;
        }
    }
}
