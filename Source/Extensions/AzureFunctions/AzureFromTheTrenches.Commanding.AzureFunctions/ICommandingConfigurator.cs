using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public interface ICommandingConfigurator
    {
        ICommandRegistry AddCommanding(ICommandingDependencyResolverAdapter adapter);
    }
}
