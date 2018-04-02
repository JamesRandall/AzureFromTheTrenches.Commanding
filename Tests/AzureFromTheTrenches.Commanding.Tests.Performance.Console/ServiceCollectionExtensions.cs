using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.Tests.Performance.Console
{
    public static class ServiceCollectionExtensions
    {
        public static CommandingDependencyResolverAdapter GetCommandingDependencyResolver(
            this IServiceCollection serviceCollection, Func<IServiceProvider> serviceProviderFunc)
        {
            return new CommandingDependencyResolverAdapter((type, instance) => serviceCollection.AddSingleton(type, instance),
                (type, impl) => serviceCollection.AddTransient(type, impl),
                type => serviceProviderFunc().GetService(type));
        }
    }
}
