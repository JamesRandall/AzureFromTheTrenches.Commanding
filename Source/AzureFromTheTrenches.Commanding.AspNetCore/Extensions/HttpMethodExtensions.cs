using System;
using System.Net.Http;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Extensions
{
    public static class HttpMethodExtensions
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

        public static string ToMethodName(this HttpMethod method)
        {
            if (method == HttpMethod.Get)
                return "Get";
            if (method == HttpMethod.Post)
                return "Post";
            if (method == HttpMethod.Put)
                return "Put";
            if (method == HttpMethod.Delete)
                return "Delete";
            throw new NotSupportedException($"Verb {method} is not supported");
        }
    }
}
