using System;
using AccidentalFish.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AccidentalFish.Commanding.Tests.Performance.Console
{
    public static class ServiceCollectionExtensions
    {
        public static CommandingDependencyResolver GetCommandingDependencyResolver(
            this IServiceCollection serviceCollection, Func<IServiceProvider> serviceProviderFunc)
        {
            return new CommandingDependencyResolver((type, instance) => serviceCollection.AddSingleton(type, instance),
                (type, impl) => serviceCollection.AddTransient(type, impl),
                type => serviceProviderFunc().GetService(type));
        }
    }
}
