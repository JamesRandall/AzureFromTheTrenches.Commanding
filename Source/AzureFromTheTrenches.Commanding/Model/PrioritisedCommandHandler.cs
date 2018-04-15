using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Model
{
    internal sealed class PrioritisedCommandHandler : IComparable, IPrioritisedCommandHandler
    {
        internal PrioritisedCommandHandler(int priority, Type commandHandlerType)
        {
            Priority = priority;
            CommandHandlerType = commandHandlerType;
        }

        public int Priority { get; }

        public Type CommandHandlerType { get; }

        public int CompareTo(object obj)
        {
            PrioritisedCommandHandler otherHandler = (PrioritisedCommandHandler)obj;
            int priorityComparison = Priority.CompareTo(otherHandler.Priority);
            if (priorityComparison != 0)
            {
                return priorityComparison;
            }
            return String.Compare(CommandHandlerType.FullName, otherHandler.CommandHandlerType.FullName, StringComparison.Ordinal);
        }
    }
}
