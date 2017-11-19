using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Http.Implementation;

namespace AzureFromTheTrenches.Commanding.Http
{
    public static class HttpCommandingDependencies
    {
        public static ICommandingDependencyResolver UseHttpCommanding(this ICommandingDependencyResolver dependencyResolver)
        {
            Register<JsonCommandSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        public static ICommandingDependencyResolver UseHttpCommanding<TSerializer>(this ICommandingDependencyResolver dependencyResolver) where TSerializer : IHttpCommandSerializer
        {
            Register<TSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        private static ICommandingDependencyResolver Register<TSerializer>(ICommandingDependencyResolver dependencyResolver) where TSerializer : IHttpCommandSerializer
        {
            dependencyResolver.TypeMapping<IHttpCommandSerializer, TSerializer>();
            dependencyResolver.TypeMapping<IUriCommandQueryBuilder, UriCommandQueryBuilder>();
            dependencyResolver.TypeMapping<IHttpCommandDispatcherFactory, HttpCommandDispatcherFactory>();
            return dependencyResolver;
        }
    }
}
