using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Implementation
{
    internal class FunctionHostBuilder : IFunctionHostBuilder
    {
        public IFunctionHostBuilder Services(Action<IServiceCollection> services)
        {
            throw new NotImplementedException();
        }

        public IFunctionHostBuilder Register(Action<ICommandRegistry> registry)
        {
            throw new NotImplementedException();
        }

        public IFunctionHostBuilder HttpFunction<TCommand>() where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionHostBuilder HttpFunction<TCommand>(string name) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionHostBuilder HttpFunction<TCommand>(Action<IHttpFunctionBuilder> httpFunctionBuilder) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionHostBuilder HttpFunction<TCommand>(string name, Action<IHttpFunctionBuilder> httpFunctionBuilder) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionHostBuilder StorageQueueFunction<TCommand>() where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionHostBuilder StorageQueueFunction<TCommand>(string functionName) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionHostBuilder StorageQueueFunction<TCommand>(Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionHostBuilder StorageQueueFunction<TCommand>(string functionName, Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }
    }
}
