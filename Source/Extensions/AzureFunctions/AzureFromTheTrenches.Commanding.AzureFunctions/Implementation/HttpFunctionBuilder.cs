using System.Net.Http;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Implementation
{
    internal class HttpFunctionBuilder : IHttpFunctionBuilder
    {
        private readonly HttpFunctionDefinition _definition;

        public HttpFunctionBuilder(HttpFunctionDefinition definition)
        {
            _definition = definition;
        }

        public IHttpFunctionBuilder AddVerb(HttpMethod verb)
        {
            _definition.Verbs.Add(verb);
            return this;
        }
    }
}
