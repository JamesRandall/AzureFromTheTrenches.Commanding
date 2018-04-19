using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public interface IHttpFunctionBuilder
    {
        IHttpFunctionBuilder AddVerb(HttpMethod verb);
    }
}
