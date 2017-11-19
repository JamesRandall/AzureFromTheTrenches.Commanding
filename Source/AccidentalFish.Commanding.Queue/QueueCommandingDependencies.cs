using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Queue.Implementation;

namespace AccidentalFish.Commanding.Queue
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
