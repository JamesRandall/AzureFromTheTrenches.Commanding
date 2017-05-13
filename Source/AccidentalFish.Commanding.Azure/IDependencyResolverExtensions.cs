using AccidentalFish.Commanding.AzureStorage.Implementation;
using AccidentalFish.DependencyResolver;

namespace AccidentalFish.Commanding.AzureStorage
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseAzureStorageCommanding<TSerializer>(this IDependencyResolver dependencyResolver) where TSerializer : IAzureStorageQueueCommandSerializer
        {
            Register<TSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        public static IDependencyResolver UseAzureStorageCommanding(this IDependencyResolver dependencyResolver)
        {
            Register<JsonCommandSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        private static void Register<TSerializer>(this IDependencyResolver dependencyResolver) where TSerializer : IAzureStorageQueueCommandSerializer
        {
            dependencyResolver.Register<IAzureStorageQueueCommandSerializer, TSerializer>();
            dependencyResolver.Register<IAzureStorageCommandQueueProcessorFactory, AzureStorageCommandQueueProcessorFactory>();
            dependencyResolver.Register<IAzureStorageQueueDispatcherFactory, AzureStorageQueueDispatcherFactory>();
        }
    }
}
