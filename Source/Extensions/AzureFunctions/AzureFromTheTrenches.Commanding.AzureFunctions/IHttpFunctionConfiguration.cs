using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public interface IHttpFunctionConfiguration
    {
        IHttpFunctionConfiguration AddVerb(HttpMethod verb);
    }
}
