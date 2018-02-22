using System.Net.Http;

namespace AzureFromTheTrenches.Commanding.Http.Implementation
{
    internal interface IHttpClientProvider
    {
        HttpClient Client { get; }
    }
}