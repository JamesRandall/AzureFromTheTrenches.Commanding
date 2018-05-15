using System.Net.Http;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Builders
{
    internal class HttpFunctionConfiguration : IHttpFunctionConfiguration
    {
        private readonly HttpFunctionDefinition _definition;

        public HttpFunctionConfiguration(HttpFunctionDefinition definition)
        {
            _definition = definition;
        }

        public IHttpFunctionConfiguration AddVerb(HttpMethod verb)
        {
            _definition.Verbs.Add(verb);
            return this;
        }        
    }
}
