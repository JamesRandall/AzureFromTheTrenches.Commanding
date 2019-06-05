using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.RabbitMQ.Implementation;

namespace AzureFromTheTrenches.Commanding.RabbitMQ
{
    public static class RabbitMqCommandingDependencies
    {
        public static ICommandingDependencyResolverAdapter AddRabbitMq(this ICommandingDependencyResolverAdapter dependencyResolver)
        {
            dependencyResolver.TypeMapping<IRabbitMQMessageSerializer, RabbitMQMessageSerializer>();
            dependencyResolver.TypeMapping<IRabbitMQQueueProcessorFactory, RabbitMQQueueProcessorFactory>();
            dependencyResolver.TypeMapping<IQueueClient, QueueClient>();
            return dependencyResolver;
        }

        public static ICommandingDependencyResolverAdapter AddAsyncMessageHandlerSingleton<T>(this ICommandingDependencyResolverAdapter services) where T : class, ICommandHandler =>
        services.TypeMapping<ICommandHandler, T>();
    }
}
