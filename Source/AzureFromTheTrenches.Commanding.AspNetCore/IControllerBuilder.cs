using System;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public interface IControllerBuilder
    {
        IControllerBuilder Controller(string controller, Action<IActionBuilder> actionBuilder);

        IControllerBuilder Controller(string controller,
            Action<IAttributeBuilder> attributeBuilder,
            Action<IActionBuilder> actionBuilder);

        IControllerBuilder Controller(string controller,
            string route,
            Action<IAttributeBuilder> attributeBuilder,
            Action<IActionBuilder> actionBuilder);
    }
}
