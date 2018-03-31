using System;
using System.Net.Http;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public interface IControllerBuilder
    {
        IControllerBuilder Controller(string controller, Action<IActionBuilder> actionBuilder); // TODO: add filter array here
    }
}
