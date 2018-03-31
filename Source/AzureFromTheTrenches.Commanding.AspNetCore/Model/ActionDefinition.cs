using System.Net.Http;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Model
{
    public class ActionDefinition
    {
        public HttpMethod Verb { get; set; }

        public string OptionalActionName { get; set; }
    }
}
