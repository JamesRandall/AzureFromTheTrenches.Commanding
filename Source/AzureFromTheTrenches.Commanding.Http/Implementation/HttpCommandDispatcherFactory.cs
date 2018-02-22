using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Http.Implementation
{
    internal class HttpCommandDispatcherFactory : IHttpCommandDispatcherFactory
    {
        private readonly IHttpCommandSerializer _httpCommandSerializer;
        private readonly IUriCommandQueryBuilder _uriCommandQueryBuilder;
        private readonly IHttpClientProvider _httpClientProvider;
        
        public HttpCommandDispatcherFactory(IHttpCommandSerializer httpCommandSerializer,
            IUriCommandQueryBuilder uriCommandQueryBuilder,
            IHttpClientProvider httpClientProvider)
        {
            _httpCommandSerializer = httpCommandSerializer;
            _uriCommandQueryBuilder = uriCommandQueryBuilder;
            _httpClientProvider = httpClientProvider;
        }

        public ICommandDispatcher Create(Uri uri, HttpMethod httpMethod = null, Func<string> authenticationHeaderContent = null)
        {
            return new HttpCommandDispatcher(new HttpCommandExecuter(
                uri,
                httpMethod,
                authenticationHeaderContent,
                _httpCommandSerializer,
                _uriCommandQueryBuilder,
                _httpClientProvider));
        }
    }
}
