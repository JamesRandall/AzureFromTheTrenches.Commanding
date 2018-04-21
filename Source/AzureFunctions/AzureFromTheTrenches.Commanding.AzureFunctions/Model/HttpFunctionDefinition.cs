using System.Collections.Generic;
using System.Net.Http;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Model
{
    public class HttpFunctionDefinition : AbstractFunctionDefinition
    {
        public HashSet<HttpMethod> Verbs { get; set; } = new HashSet<HttpMethod>();
    }
}
