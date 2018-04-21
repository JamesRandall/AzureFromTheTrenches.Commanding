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

        static Runtime()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            CommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => serviceCollection.AddSingleton(fromType, toInstance),
                (fromType, toType) => serviceCollection.AddTransient(fromType, toType),
                (resolveType) => ServiceProvider.GetService(resolveType)
            );

            ICommandRegistry commandRegistry;
            IFunctionAppConfiguration configuration = FindConfiguration();
            if (configuration is ICommandingConfigurator commandingConfigurator)
            {
                commandRegistry = commandingConfigurator.AddCommanding(adapter);
            }
            else
            {
                commandRegistry = adapter.AddCommanding();
            }

            FunctionHostBuilder builder = new FunctionHostBuilder(serviceCollection, commandRegistry, new FunctionBuilder());
            configuration.Build(builder);
            
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public static IFunctionAppConfiguration FindConfiguration()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                IFunctionAppConfiguration configuration = FindConfiguration(assembly);
                if (configuration != null)
                {
                    return configuration;
                }
            }
            throw new ConfigurationException("Unable to find implementation of IFunctionHostBuilder");
        }

        public static IFunctionAppConfiguration FindConfiguration(Assembly assembly)
        {
            Type interfaceType = typeof(IFunctionAppConfiguration);
            Type foundType = assembly.GetTypes().FirstOrDefault(x => interfaceType.IsAssignableFrom(interfaceType) && x.IsClass);
            if (foundType != null)
            {
                return (IFunctionAppConfiguration)Activator.CreateInstance(foundType);
            }

            return null;
        }

        public static ICommandDispatcher CommandDispatcher => ServiceProvider.GetService<ICommandDispatcher>();
    }
}
