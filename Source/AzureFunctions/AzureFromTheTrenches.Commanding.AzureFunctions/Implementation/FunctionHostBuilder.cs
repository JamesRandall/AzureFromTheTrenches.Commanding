using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Implementation
{
    internal class FunctionHostBuilder : IFunctionHostBuilder
    {
        public IFunctionHostBuilder HttpFunction<TCommand>(string name,
            Action<IHttpFunctionBuilder> httpFunctionBuilder) where TCommand : ICommand
        {

            return this;
        }

        public IFunctionHostBuilder StorageQueueFunction<TCommand>(string functionName) where TCommand : ICommand
        {
            return this;
        }
    }
}
