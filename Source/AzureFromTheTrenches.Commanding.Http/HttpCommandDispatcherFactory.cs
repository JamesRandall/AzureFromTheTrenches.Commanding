using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Http.Implementation;

namespace AzureFromTheTrenches.Commanding.Http
{
    /// <summary>
    /// Creates HTTP command dispatcher functions
    /// </summary>
    public static class HttpCommandDispatcherFactory
    {
        internal static readonly IHttpClientProvider HttpClientProvider = new HttpClientProvider();

        /// <summary>
        /// Will create a HTTP command dispatcher. Note if you've supplied a HttpClient to the AddHttpCommanding methods then you
        /// should use resolve an instance if IHttpCommandDispatcherFactory and use that instances create method to create a
        /// command dispatcher
        /// </summary>
        /// <param name="uri">The uri the command should be sent to</param>
        /// <param name="httpMethod">The verb to send the command with</param>
        /// <param name="authenticationHeaderContent">The content of the authentication header (null if none require)</param>
        /// <param name="serializer">An optional serializer. If unspecified the default JSON serializer will be used.</param>
        /// <param name="uriCommandQueryBuilder">An optional URI command query builder. If unspecified the default command query builder will be used.</param>
        /// <returns></returns>
        public static Func<ICommandDispatcher> Create(
            Uri uri,
            HttpMethod httpMethod = null,
            Func<string> authenticationHeaderContent = null,
            IHttpCommandSerializer serializer = null,
            IUriCommandQueryBuilder uriCommandQueryBuilder = null)
        {
            return () => 
                new HttpCommandDispatcherFactoryImpl(
                    serializer ?? new JsonCommandSerializer(),
                    uriCommandQueryBuilder ?? new UriCommandQueryBuilder(),
                    HttpClientProvider).Create(uri, httpMethod, authenticationHeaderContent);
        }
    }
}
