using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public interface IFunctionBuilder
    {
        IFunctionBuilder HttpRoute(string routePrefix, Action<IHttpFunctionBuilder> httpRouteBuilder);
        IFunctionBuilder HttpFunction<TCommand>() where TCommand : ICommand;
        IFunctionBuilder HttpFunction<TCommand>(HttpMethod method) where TCommand : ICommand;
        IFunctionBuilder HttpFunction<TCommand>(string route, HttpMethod method) where TCommand : ICommand;
        IFunctionBuilder HttpFunction<TCommand>(Action<IHttpFunctionConfiguration> httpFunctionBuilder) where TCommand : ICommand;
        IFunctionBuilder HttpFunction<TCommand>(string route, Action<IHttpFunctionConfiguration> httpFunctionBuilder) where TCommand : ICommand;

        IFunctionBuilder StorageQueueFunction<TCommand>() where TCommand : ICommand;
        IFunctionBuilder StorageQueueFunction<TCommand>(string functionName) where TCommand : ICommand;
        IFunctionBuilder StorageQueueFunction<TCommand>(Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand;
        IFunctionBuilder StorageQueueFunction<TCommand>(string functionName, Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand;
    }
}
