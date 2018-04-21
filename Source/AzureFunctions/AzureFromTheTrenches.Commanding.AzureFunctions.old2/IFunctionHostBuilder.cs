using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public interface IFunctionHostBuilder
    {
        IFunctionHostBuilder Services(Action<IServiceCollection> services);

        /// <summary>
        /// Surfaces an ICommandRegistry that allows commands and command handlers to be registered
        /// </summary>
        /// <param name="registry">The command registry</param>
        /// <returns>The function host builder to support a Fluent API</returns>
        IFunctionHostBuilder Register(Action<ICommandRegistry> registry);

        IFunctionHostBuilder HttpFunction<TCommand>() where TCommand : ICommand;
        IFunctionHostBuilder HttpFunction<TCommand>(string name) where TCommand : ICommand;
        IFunctionHostBuilder HttpFunction<TCommand>(Action<IHttpFunctionBuilder> httpFunctionBuilder) where TCommand : ICommand;
        IFunctionHostBuilder HttpFunction<TCommand>(string name, Action<IHttpFunctionBuilder> httpFunctionBuilder) where TCommand : ICommand;

        IFunctionHostBuilder StorageQueueFunction<TCommand>() where TCommand : ICommand;
        IFunctionHostBuilder StorageQueueFunction<TCommand>(string functionName) where TCommand : ICommand;
        IFunctionHostBuilder StorageQueueFunction<TCommand>(Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand;
        IFunctionHostBuilder StorageQueueFunction<TCommand>(string functionName, Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand;
    }
}
