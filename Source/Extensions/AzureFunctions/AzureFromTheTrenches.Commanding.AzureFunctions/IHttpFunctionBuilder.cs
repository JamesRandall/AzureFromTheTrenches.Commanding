using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public interface IHttpFunctionBuilder
    {
        IHttpFunctionBuilder HttpFunction<TCommand>() where TCommand : ICommand;
        IHttpFunctionBuilder HttpFunction<TCommand>(HttpMethod method) where TCommand : ICommand;
        IHttpFunctionBuilder HttpFunction<TCommand>(string route, HttpMethod method) where TCommand : ICommand;
        IHttpFunctionBuilder HttpFunction<TCommand>(Action<IHttpFunctionConfiguration> httpFunctionBuilder) where TCommand : ICommand;
        IHttpFunctionBuilder HttpFunction<TCommand>(string route, Action<IHttpFunctionConfiguration> httpFunctionBuilder) where TCommand : ICommand;
    }
}
