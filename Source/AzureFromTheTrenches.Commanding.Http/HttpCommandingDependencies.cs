using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Http.Implementation;

namespace AzureFromTheTrenches.Commanding.Http
{
    public static class HttpCommandingDependencies
    {
        internal static IHttpClientProvider HttpClientProvider;

        public static ICommandingDependencyResolver UseHttpCommanding(this ICommandingDependencyResolver dependencyResolver, HttpClient client = null)
        {
            Register<JsonCommandSerializer>(dependencyResolver, client);
            return dependencyResolver;
        }

        public static ICommandingDependencyResolver UseHttpCommanding<TSerializer>(this ICommandingDependencyResolver dependencyResolver, HttpClient client = null) where TSerializer : IHttpCommandSerializer
        {
            Register<TSerializer>(dependencyResolver, client);
            return dependencyResolver;
        }

        private static ICommandingDependencyResolver Register<TSerializer>(ICommandingDependencyResolver dependencyResolver, HttpClient client) where TSerializer : IHttpCommandSerializer
        {
            HttpClientProvider = client == null ? new HttpClientProvider() : new HttpClientProvider(client);
            dependencyResolver.RegisterInstance(HttpClientProvider);
            dependencyResolver.TypeMapping<IHttpCommandSerializer, TSerializer>();
            dependencyResolver.TypeMapping<IUriCommandQueryBuilder, UriCommandQueryBuilder>();
            dependencyResolver.TypeMapping<IHttpCommandDispatcherFactory, HttpCommandDispatcherFactory>();
            return dependencyResolver;
        }
    }
}
