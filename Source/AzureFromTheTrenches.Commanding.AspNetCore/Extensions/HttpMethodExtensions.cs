using System;
using System.Net.Http;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Extensions
{
    internal static class HttpMethodExtensions
    {
        public static string ToControllerAttribute(this HttpMethod method)
        {
            if (method == HttpMethod.Get)
                return "[HttpGet]";
            if (method == HttpMethod.Post)
                return "[HttpPost]";
            if (method == HttpMethod.Put)
                return "[HttpPut]";
            if (method == HttpMethod.Delete)
                return "[HttpDelete]";
            throw new NotSupportedException($"Verb {method} is not supported");
        }
    }
}
