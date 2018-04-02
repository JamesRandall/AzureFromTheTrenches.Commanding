using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection
{
    public interface IMicrosoftDependencyInjectionCommandingResolverAdapter : ICommandingDependencyResolverAdapter
    {
        ICommandRegistry Registry { get; }

        IServiceProvider ServiceProvider { get; set; }
    }
}
