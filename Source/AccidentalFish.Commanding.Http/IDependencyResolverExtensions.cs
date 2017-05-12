using AccidentalFish.Commanding.Http.Implementation;
using AccidentalFish.DependencyResolver;

namespace AccidentalFish.Commanding.Http
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static IDependencyResolver UseHttpCommanding(this IDependencyResolver dependencyResolver)
        {
            Register<JsonCommandSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        public static IDependencyResolver UseHttpCommanding<TSerializer>(this IDependencyResolver dependencyResolver) where TSerializer : IHttpCommandSerializer
        {
            Register<TSerializer>(dependencyResolver);
            return dependencyResolver;
        }

        private static void Register<TSerializer>(IDependencyResolver dependencyResolver) where TSerializer : IHttpCommandSerializer
        {
            dependencyResolver.Register<IHttpCommandSerializer, TSerializer>();
            dependencyResolver.Register<IUriCommandQueryBuilder, UriCommandQueryBuilder>();
            dependencyResolver.Register<IHttpCommandDispatcherFactory, HttpCommandDispatcherFactory>();
        }
    }
}
