using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation
{
    interface IControllerCompiler
    {
        void Compile(IReadOnlyCollection<ControllerDefinition> definitions);
    }
}
