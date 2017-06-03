using AccidentalFish.DependencyResolver;
using AccidentalFish.Commanding.Queue.Implementation;

namespace AccidentalFish.Commanding.Queue
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseQueues(this IDependencyResolver dependencyResolver)
        {
            dependencyResolver.Register<IAsynchronousBackoffPolicyFactory, AsynchronousBackoffPolicyFactory>();
            dependencyResolver.Register<ICommandQueueProcessor, CommandQueueProcessor>();

            return dependencyResolver;
        }
    }
}
