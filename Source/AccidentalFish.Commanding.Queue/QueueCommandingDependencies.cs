using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.DependencyResolver;
using AccidentalFish.Commanding.Queue.Implementation;

namespace AccidentalFish.Commanding.Queue
{
    // ReSharper disable once InconsistentNaming
    public static class QueueCommandingDependencies
    {
        public static void UseQueues(CommandingDependencyResolver dependencyResolver)
        {
            dependencyResolver.TypeMapping<IAsynchronousBackoffPolicyFactory, AsynchronousBackoffPolicyFactory>();
            dependencyResolver.TypeMapping<ICommandQueueProcessor, CommandQueueProcessor>();            
        }
    }
}
