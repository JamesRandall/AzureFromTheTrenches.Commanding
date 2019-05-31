using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Http.Implementation
{
    internal class HttpCommandExecuter : ICommandExecuter
    {
        private readonly Uri _uri;
        private readonly HttpMethod _httpMethod;
        private readonly Func<string> _authenticationHeaderContent;
        private readonly IHttpCommandSerializer _serializer;
        private readonly IUriCommandQueryBuilder _uriCommandQueryBuilder;
        private readonly IHttpClientProvider _httpClientProvider;
        private readonly HttpDispatchErrorHandler _httpDispatchErrorHandler;

        public HttpCommandExecuter(Uri uri,
            HttpMethod httpMethod,
            Func<string> authenticationHeaderContent,
            IHttpCommandSerializer serializer,
            IUriCommandQueryBuilder uriCommandQueryBuilder,
            IHttpClientProvider httpClientProvider,
            HttpDispatchErrorHandler httpDispatchErrorHandler)
        {
            _uri = uri;
            _httpMethod = httpMethod ?? HttpMethod.Post;
            _authenticationHeaderContent = authenticationHeaderContent;
            _serializer = serializer;
            _uriCommandQueryBuilder = uriCommandQueryBuilder;
            _httpClientProvider = httpClientProvider;
            _httpDispatchErrorHandler = httpDispatchErrorHandler;
        }

        public async Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
        {
            string json = _serializer.Serialize(command);
            
            HttpRequestMessage requestMessage;
            if (_httpMethod == HttpMethod.Post || _httpMethod == HttpMethod.Put)
            {
                requestMessage = new HttpRequestMessage
                {
                    Content = new StringContent(json, Encoding.UTF8, _serializer.MimeType),
                    Method = _httpMethod,
                    Headers = {Accept = {MediaTypeWithQualityHeaderValue.Parse(_serializer.MimeType)}},
                    RequestUri = _uri
                };                
            }
            else
            {
                UriBuilder uriBuilder = new UriBuilder(_uri) {Query = _uriCommandQueryBuilder.Query(_uri, command)};
                requestMessage = new HttpRequestMessage
                {
                    Method = _httpMethod,
                    Headers = {Accept = {MediaTypeWithQualityHeaderValue.Parse(_serializer.MimeType)}},
                    RequestUri = uriBuilder.Uri
                };
            }

            if (_authenticationHeaderContent != null)
            {
                requestMessage.Headers.Authorization = AuthenticationHeaderValue.Parse(_authenticationHeaderContent());
            }

            HttpResponseMessage responseMessage = await _httpClientProvider.Client.SendAsync(requestMessage, cancellationToken);
            if (!responseMessage.IsSuccessStatusCode)
            {
                if (_httpDispatchErrorHandler != null)
                {
                    Exception exception = _httpDispatchErrorHandler.Invoke(responseMessage.StatusCode, requestMessage.RequestUri, command);
                    if (exception != null)
                    {
                        throw exception;
                    }
                }
                
                // if we don't throw an exception from the custom handler then have the build in HTTP error handling
                // kick in
                responseMessage.EnsureSuccessStatusCode();
            }
            
            string result = await responseMessage.Content.ReadAsStringAsync();
            return _serializer.Deserialize<TResult>(result);
        }
    }
}
