using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Http.Implementation;

namespace AzureFromTheTrenches.Commanding.Http
{
    // ReSharper disable once InconsistentNaming
    public static class ICommandRegistryExtensions
    {
        public static void RegisterHttpCommand<TCommand, TResult>(this ICommandRegistry registry,
            Uri uri,
            HttpMethod httpMethod = null,
            Func<string> authenticationHeaderContent = null,
            IHttpCommandSerializer httpCommandSerializer = null,
            IUriCommandQueryBuilder uriCommandQueryBuilder = null,
            HttpDispatchErrorHandler httpDispatchErrorHandler = null) where TCommand : ICommand<TResult>
        {
            registry.Register<TCommand, TResult>(() => new HttpCommandDispatcher(new HttpCommandExecuter(
                uri,
                httpMethod, authenticationHeaderContent, httpCommandSerializer ?? new JsonCommandSerializer(),
                uriCommandQueryBuilder ?? new UriCommandQueryBuilder(),
                HttpCommandingDependencies.HttpClientProvider,
                httpDispatchErrorHandler)));
        }
    }
}
