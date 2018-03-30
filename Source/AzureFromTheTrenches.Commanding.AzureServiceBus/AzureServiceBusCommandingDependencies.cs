using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureServiceBus.Implementation;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus
{
    public static class AzureServiceBusCommandingDependencies
    {
        public static ICommandingDependencyResolver UseAzureServiceBus(this ICommandingDependencyResolver dependencyResolver)
        {
            dependencyResolver.TypeMapping<IServiceBusMessageSerializer, JsonServiceBusMessageSerializer>();
            dependencyResolver.TypeMapping<IServiceBusCommandQueueProcessorFactory, ServiceBusCommandQueueProcessorFactory>();
            return dependencyResolver;
        }
    }
}
