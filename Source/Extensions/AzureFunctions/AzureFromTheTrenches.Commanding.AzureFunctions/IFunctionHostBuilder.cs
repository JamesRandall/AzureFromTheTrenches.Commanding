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
        /// <param name="services">An action that will be given a command registry and service collection</param>
        /// <returns>The function host builder for use in a Fluent API</returns>
        IFunctionHostBuilder Setup(Action<IServiceCollection, ICommandRegistry> services);

        /// <summary>
        /// Surfaces a builder for configurating authorization
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        IFunctionHostBuilder Authorization(Action<IAuthorizationBuilder> authorization);

        /// <summary>
        /// Surfaces a builder for declaring the command to function bindings
        /// </summary>
        /// <param name="functions">The function builder</param>
        /// <returns>The function host builder to support a Fluent API</returns>
        IFunctionHostBuilder Functions(Action<IFunctionBuilder> functions);
    }
}
