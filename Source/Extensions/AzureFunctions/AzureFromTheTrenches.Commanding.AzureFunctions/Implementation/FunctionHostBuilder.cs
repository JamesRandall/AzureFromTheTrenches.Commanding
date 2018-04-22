using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Implementation
{
    public class FunctionHostBuilder : IFunctionHostBuilder
    {
        public IServiceCollection ServiceCollection { get; }
        public ICommandRegistry CommandRegistry { get; }
        public IFunctionBuilder FunctionBuilder { get; }

        public FunctionHostBuilder(
            IServiceCollection serviceCollection,
            ICommandRegistry commandRegistry)
        {
            ServiceCollection = serviceCollection;
            CommandRegistry = commandRegistry;
            FunctionBuilder = new FunctionBuilder();
        }

        public IFunctionHostBuilder Setup(Action<IServiceCollection, ICommandRegistry> services)
        {
            services(ServiceCollection, CommandRegistry);
            return this;
        }        

        public IFunctionHostBuilder Functions(Action<IFunctionBuilder> functions)
        {
            functions(FunctionBuilder);
            return this;
        }

        public IReadOnlyCollection<AbstractFunctionDefinition> FunctionDefinitions => ((FunctionBuilder)FunctionBuilder).Definitions;
    }
}
