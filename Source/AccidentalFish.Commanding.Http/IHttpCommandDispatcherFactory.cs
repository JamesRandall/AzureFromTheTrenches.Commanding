using System;
using System.Net.Http;

namespace AccidentalFish.Commanding.Http
{
    public interface IHttpCommandDispatcherFactory
    {
        ICommandDispatcher Create(Uri uri, HttpMethod httpMethod = null, Func<string> authenticationHeaderContent = null);
    }
}
