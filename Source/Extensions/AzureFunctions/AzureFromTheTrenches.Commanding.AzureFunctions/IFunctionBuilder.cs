using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public interface IFunctionBuilder
    {
        IFunctionBuilder HttpFunction<TCommand>() where TCommand : ICommand;
        IFunctionBuilder HttpFunction<TCommand>(string name) where TCommand : ICommand;
        IFunctionBuilder HttpFunction<TCommand>(Action<IHttpFunctionBuilder> httpFunctionBuilder) where TCommand : ICommand;
        IFunctionBuilder HttpFunction<TCommand>(string name, Action<IHttpFunctionBuilder> httpFunctionBuilder) where TCommand : ICommand;

        IFunctionBuilder StorageQueueFunction<TCommand>() where TCommand : ICommand;
        IFunctionBuilder StorageQueueFunction<TCommand>(string functionName) where TCommand : ICommand;
        IFunctionBuilder StorageQueueFunction<TCommand>(Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand;
        IFunctionBuilder StorageQueueFunction<TCommand>(string functionName, Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand;
    }
}
