using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Queue.Implementation;

namespace AzureFromTheTrenches.Commanding.Queue
{
    // ReSharper disable once InconsistentNaming
    public static class QueueCommandingDependencies
    {
        public static ICommandingDependencyResolver UseQueues(this ICommandingDependencyResolver dependencyResolver)
        {
            dependencyResolver.TypeMapping<IAsynchronousBackoffPolicyFactory, AsynchronousBackoffPolicyFactory>();
            dependencyResolver.TypeMapping<ICommandQueueProcessor, CommandQueueProcessor>();
            return dependencyResolver;
        }
    }
}
