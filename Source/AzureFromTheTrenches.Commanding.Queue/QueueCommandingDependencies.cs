using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Queue.Implementation;

namespace AzureFromTheTrenches.Commanding.Queue
{
    // ReSharper disable once InconsistentNaming
    public static class QueueCommandingDependencies
    {
        [Obsolete("Please use AddQueues instead")]
        public static ICommandingDependencyResolver UseQueues(this ICommandingDependencyResolver dependencyResolver,
            Action<string, ICommand, Exception> logError=null,
            Action<string, ICommand, Exception> logWarning = null,
            Action<string, ICommand, Exception> logInfo = null)
        {
            ICommandQueueProcessorLogger logger = new CommandQueueProcessorLogger(logWarning, logError, logInfo);
            dependencyResolver.RegisterInstance(logger);
            dependencyResolver.TypeMapping<IAsynchronousBackoffPolicyFactory, AsynchronousBackoffPolicyFactory>();
            dependencyResolver.TypeMapping<ICommandQueueProcessor, CommandQueueProcessor>();
            return dependencyResolver;
        }

        public static ICommandingDependencyResolverAdapter AddQueues(this ICommandingDependencyResolverAdapter dependencyResolver,
            Action<string, ICommand, Exception> logError = null,
            Action<string, ICommand, Exception> logWarning = null,
            Action<string, ICommand, Exception> logInfo = null)
        {
            ICommandQueueProcessorLogger logger = new CommandQueueProcessorLogger(logWarning, logError, logInfo);
            dependencyResolver.RegisterInstance(logger);
            dependencyResolver.TypeMapping<IAsynchronousBackoffPolicyFactory, AsynchronousBackoffPolicyFactory>();
            dependencyResolver.TypeMapping<ICommandQueueProcessor, CommandQueueProcessor>();
            return dependencyResolver;
        }
    }
}
