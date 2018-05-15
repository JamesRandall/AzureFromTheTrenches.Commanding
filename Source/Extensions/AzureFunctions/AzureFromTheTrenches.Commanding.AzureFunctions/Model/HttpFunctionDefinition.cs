using System.Collections.Generic;
using System.Net.Http;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Model
{
    public class HttpFunctionDefinition : AbstractFunctionDefinition
    {
        public HashSet<HttpMethod> Verbs { get; set; } = new HashSet<HttpMethod>();

        public AuthorizationTypeEnum? Authorization { get; set; }

        public bool ValidatesToken { get; set; }

        // used to create a proxy that maps through to the internal function
        public string Route { get; set; }

        public IReadOnlyCollection<HttpQueryParameter> AcceptsQueryParameters { get; set; }
    }
}
