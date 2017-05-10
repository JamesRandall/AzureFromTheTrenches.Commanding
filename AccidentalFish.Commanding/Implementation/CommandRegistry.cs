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

        public void Register<T1, T2>(ICommandDispatcher dispatcher = null, int order = CommandActorOrder.Default) where T1 : class where T2 : ICommandActorBase<T1>
        {
            SortedSet<PrioritisedCommandActor> set;
            if (!_actors.TryGetValue(typeof(T1), out set))
            {
                set = new SortedSet<PrioritisedCommandActor>();
                _actors.Add(typeof(T1), set);
            }

            set.Add(new PrioritisedCommandActor(order, typeof(T2)));
            if (dispatcher != null)
            {
                _commandDispatchers[typeof(T1)] = dispatcher;
            }
        }

        public IReadOnlyCollection<PrioritisedCommandActor> GetPrioritisedCommandActors<T>() where T : class
        {
            SortedSet<PrioritisedCommandActor> set;
            if (!_actors.TryGetValue(typeof(T), out set))
            {
                throw new MissingCommandActorRegistrationException(typeof(T));
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
