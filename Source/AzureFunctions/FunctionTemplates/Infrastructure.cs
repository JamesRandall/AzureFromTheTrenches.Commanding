using System;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionTemplates.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionTemplates
{
    internal static class Infrastructure
    {
        private static readonly IServiceProvider ServiceProvider;
        
        static Infrastructure()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            CommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => serviceCollection.AddSingleton(fromType, toInstance),
                (fromType, toType) => serviceCollection.AddTransient(fromType, toType),
                (resolveType) => ServiceProvider.GetService(resolveType)
                );

            ICommandRegistry registry = adapter.AddCommanding();
            registry.Register<EchoMessageCommandHandler>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public static ICommandDispatcher CommandDispatcher => ServiceProvider.GetService<ICommandDispatcher>();
    }
}
