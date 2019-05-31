using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Http
{
    public interface IHttpCommandDispatcherFactory
    {
        ICommandDispatcher Create(Uri uri,
            HttpMethod httpMethod = null,
            Func<string> authenticationHeaderContent = null,
            HttpDispatchErrorHandler httpDispatchErrorHandler = null);
    }
}
