using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public static class Runtime
    {
        private static readonly IServiceProvider ServiceProvider;
        private static readonly IServiceCollection ServiceCollection;

        static Runtime()
        {
            ServiceCollection = new ServiceCollection();
            CommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => ServiceCollection.AddSingleton(fromType, toInstance),
                (fromType, toType) => ServiceCollection.AddTransient(fromType, toType),
                (resolveType) => ServiceProvider.GetService(resolveType)
            );

            ICommandRegistry commandRegistry;
            IFunctionAppConfiguration configuration = ConfigurationLocator.FindConfiguration();
            if (configuration is ICommandingConfigurator commandingConfigurator)
            {
                commandRegistry = commandingConfigurator.AddCommanding(adapter);
            }
            else
            {
                commandRegistry = adapter.AddCommanding();
            }

            FunctionHostBuilder builder = new FunctionHostBuilder(ServiceCollection, commandRegistry);
            configuration.Build(builder);
            
            ServiceProvider = ServiceCollection.BuildServiceProvider();
        }

        public static ICommandDispatcher CommandDispatcher => ServiceProvider.GetService<ICommandDispatcher>();
    }
}
