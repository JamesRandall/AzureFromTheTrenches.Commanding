using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Http.Implementation;

namespace AccidentalFish.Commanding.Http
{
    public static class HttpCommandingDependencies
    {
        public static void UseHttpCommanding(CommandingDependencyResolver dependencyResolver)
        {
            Register<JsonCommandSerializer>(dependencyResolver);
        }

        public static void UseHttpCommanding<TSerializer>(CommandingDependencyResolver dependencyResolver) where TSerializer : IHttpCommandSerializer
        {
            Register<TSerializer>(dependencyResolver);
        }

        private static void Register<TSerializer>(CommandingDependencyResolver dependencyResolver) where TSerializer : IHttpCommandSerializer
        {
            dependencyResolver.TypeMapping<IHttpCommandSerializer, TSerializer>();
            dependencyResolver.TypeMapping<IUriCommandQueryBuilder, UriCommandQueryBuilder>();
            dependencyResolver.TypeMapping<IHttpCommandDispatcherFactory, HttpCommandDispatcherFactory>();
        }
    }
}
