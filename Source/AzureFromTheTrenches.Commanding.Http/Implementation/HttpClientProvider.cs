using System.Net.Http;

namespace AzureFromTheTrenches.Commanding.Http.Implementation
{
    class HttpClientProvider : IHttpClientProvider
    {
        public  HttpClient Client { get; }

        public HttpClientProvider() : this(new HttpClient())
        {
            
        }

        public HttpClientProvider(HttpClient client)
        {
            Client = client;
        }
    }
}
