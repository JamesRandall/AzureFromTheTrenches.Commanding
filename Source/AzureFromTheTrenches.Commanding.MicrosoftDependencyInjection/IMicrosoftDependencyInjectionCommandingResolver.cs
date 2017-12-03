using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection
{
    public interface IMicrosoftDependencyInjectionCommandingResolver : ICommandingDependencyResolver
    {
        ICommandRegistry Registry { get; }

        IServiceProvider ServiceProvider { get; set; }
    }
}
