using System;
using System.Net;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    /// <summary>
    /// This class is designed to be thrown by a mediator to influence the response of the controllers
    /// when an unexpected event occurs
    /// </summary>
    public class RestApiException : Exception
    {
        public RestApiException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public RestApiException(HttpStatusCode statusCode, object payload)
        {
            StatusCode = statusCode;
            Payload = payload;
        }

        public HttpStatusCode StatusCode { get; }

        public object Payload { get; }
    }
}
