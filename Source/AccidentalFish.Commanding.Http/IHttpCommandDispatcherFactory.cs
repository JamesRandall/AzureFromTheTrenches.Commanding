using System;
using System.Net.Http;
using AccidentalFish.Commanding.Abstractions;

namespace AccidentalFish.Commanding.Http
{
    public interface IHttpCommandDispatcherFactory
    {
        ICommandDispatcher Create(Uri uri, HttpMethod httpMethod = null, Func<string> authenticationHeaderContent = null);
    }
}
