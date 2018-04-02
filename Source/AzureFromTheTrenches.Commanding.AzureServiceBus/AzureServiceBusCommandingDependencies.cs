using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureServiceBus.Implementation;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus
{
    public static class AzureServiceBusCommandingDependencies
    {
        public static ICommandingDependencyResolverAdapter AddAzureServiceBus(this ICommandingDependencyResolverAdapter dependencyResolver)
        {
            dependencyResolver.TypeMapping<IServiceBusMessageSerializer, JsonServiceBusMessageSerializer>();
            dependencyResolver.TypeMapping<IServiceBusCommandQueueProcessorFactory, ServiceBusCommandQueueProcessorFactory>();
            return dependencyResolver;
        }
    }
}
