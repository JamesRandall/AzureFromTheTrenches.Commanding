using System.Net.Http;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    public class ActionDefinition
    {
        public HttpMethod Verb { get; set; }

        public string OptionalActionName { get; set; }
    }
}
