using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Builders
{
    public class FunctionHostBuilder : IFunctionHostBuilder
    {
        public IServiceCollection ServiceCollection { get; }
        public ICommandRegistry CommandRegistry { get; }
        public IFunctionBuilder FunctionBuilder { get; } = new FunctionBuilder();
        public IAuthorizationBuilder AuthorizationBuilder { get; } = new AuthorizationBuilder();

        public FunctionHostBuilder(
            IServiceCollection serviceCollection,
            ICommandRegistry commandRegistry)
        {
            ServiceCollection = serviceCollection;
            CommandRegistry = commandRegistry;
        }

        public IFunctionHostBuilder Setup(Action<IServiceCollection, ICommandRegistry> services)
        {
            services(ServiceCollection, CommandRegistry);
            return this;
        }

        public IFunctionHostBuilder Authorization(Action<IAuthorizationBuilder> authorization)
        {
            authorization(AuthorizationBuilder);
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
