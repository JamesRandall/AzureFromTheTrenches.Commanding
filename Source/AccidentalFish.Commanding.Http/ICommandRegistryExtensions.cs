using System;
using System.Net.Http;
using AccidentalFish.Commanding.Http.Implementation;

namespace AccidentalFish.Commanding.Http
{
    // ReSharper disable once InconsistentNaming
    public static class ICommandRegistryExtensions
    {
        public static void RegisterHttpCommand<TCommand>(this ICommandRegistry registry,
            Uri uri,
            HttpMethod httpMethod = null,
            Func<string> authenticationHeaderContent = null,
            IHttpCommandSerializer httpCommandSerializer = null,
            IUriCommandQueryBuilder uriCommandQueryBuilder = null) where TCommand : class
        {
            registry.Register<TCommand>(() => new HttpCommandDispatcher(new HttpCommandExecuter(uri, httpMethod, authenticationHeaderContent, httpCommandSerializer ?? new JsonCommandSerializer(), uriCommandQueryBuilder ?? new UriCommandQueryBuilder())));
        }
    }
}
