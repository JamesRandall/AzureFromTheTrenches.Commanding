using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public interface IFunctionHostBuilder
    {
        /// <summary>
        /// Surfaces an IServiceCollection into which dependencies (for command handlers) can be registered
        /// </summary>
        /// <param name="serviceCollection">The service collection</param>
        /// <returns>The function host builder for use in a Fluent API</returns>
        IFunctionHostBuilder Services(Action<IServiceCollection> serviceCollection);

        /// <summary>
        /// Surfaces an ICommandRegistry that allows commands and command handlers to be registered
        /// </summary>
        /// <param name="registry">The command registry</param>
        /// <returns>The function host builder to support a Fluent API</returns>
        IFunctionHostBuilder Register(Action<ICommandRegistry> registry);

        /// <summary>
        /// Surfaces a builder for declaring the command to function bindings
        /// </summary>
        /// <param name="functions">The function builder</param>
        /// <returns>The function host builder to support a Fluent API</returns>
        IFunctionHostBuilder Functions(Action<IFunctionBuilder> functions);
    }
}
