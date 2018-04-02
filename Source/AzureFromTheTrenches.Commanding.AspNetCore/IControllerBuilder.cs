using System;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public interface IControllerBuilder
    {
        IControllerBuilder Controller(string controller, Action<IActionBuilder> actionBuilder); // TODO: add filter array here

        IControllerBuilder Controller(string controller, string route, Action<IActionBuilder> actionBuilder);
    }
}
