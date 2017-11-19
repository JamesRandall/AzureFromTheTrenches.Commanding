using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Model
{
    public sealed class PrioritisedCommandActor : IComparable, IPrioritisedCommandActor
    {
        internal PrioritisedCommandActor(int priority, Type commandActorType)
        {
            Priority = priority;
            CommandActorType = commandActorType;
        }

        public int Priority { get; }

        public Type CommandActorType { get; }

        public int CompareTo(object obj)
        {
            PrioritisedCommandActor otherActor = (PrioritisedCommandActor)obj;
            int priorityComparison = Priority.CompareTo(otherActor.Priority);
            if (priorityComparison != 0)
            {
                return priorityComparison;
            }
            return String.Compare(CommandActorType.FullName, otherActor.CommandActorType.FullName, StringComparison.Ordinal);
        }
    }
}
