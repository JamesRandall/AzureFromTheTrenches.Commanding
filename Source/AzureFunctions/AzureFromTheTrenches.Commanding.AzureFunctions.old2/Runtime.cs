using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
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

            IFunctionAppConfiguration configuration = FindConfiguration();
            //ICommandRegistry registry = adapter.AddCommanding();
            
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public static ICommandDispatcher CommandDispatcher => ServiceProvider.GetService<ICommandDispatcher>();

        private static IFunctionAppConfiguration FindConfiguration()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type interfaceType = typeof(IFunctionAppConfiguration);
            foreach (Assembly assembly in assemblies)
            {
                Type foundType = assembly.GetTypes().FirstOrDefault(x => interfaceType.IsAssignableFrom(interfaceType) && x.IsClass);
                if (foundType != null)
                {
                    return (IFunctionAppConfiguration)Activator.CreateInstance(foundType);
                }
            }
            throw new ConfigurationException("Unable to find implementation of IFunctionHostBuilder");
        }
    }
}
