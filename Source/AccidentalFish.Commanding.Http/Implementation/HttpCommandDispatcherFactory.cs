using System;
using System.Net.Http;

namespace AccidentalFish.Commanding.Http.Implementation
{
    internal class HttpCommandDispatcherFactory : IHttpCommandDispatcherFactory
    {
        private readonly IHttpCommandSerializer _httpCommandSerializer;
        private readonly IUriCommandQueryBuilder _uriCommandQueryBuilder;

        public HttpCommandDispatcherFactory(IHttpCommandSerializer httpCommandSerializer, IUriCommandQueryBuilder uriCommandQueryBuilder)
        {
            _httpCommandSerializer = httpCommandSerializer;
            _uriCommandQueryBuilder = uriCommandQueryBuilder;
        }

        public ICommandDispatcher Create(Uri uri, HttpMethod httpMethod = null, Func<string> authenticationHeaderContent = null)
        {
            return new HttpCommandDispatcher(new HttpCommandExecuter(uri, httpMethod, authenticationHeaderContent, _httpCommandSerializer, _uriCommandQueryBuilder));
        }
    }
}
