using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Implementation
{
    internal class FunctionBuilder : IFunctionBuilder
    {
        public IFunctionBuilder HttpFunction<TCommand>() where TCommand : ICommand
        {
            return HttpFunction<TCommand>(null, null);
        }

        public IFunctionBuilder HttpFunction<TCommand>(string name) where TCommand : ICommand
        {
            return HttpFunction<TCommand>(name, null);
        }

        public IFunctionBuilder HttpFunction<TCommand>(Action<IHttpFunctionBuilder> httpFunctionBuilder) where TCommand : ICommand
        {
            return HttpFunction<TCommand>(null, httpFunctionBuilder);
        }

        public IFunctionBuilder HttpFunction<TCommand>(string name, Action<IHttpFunctionBuilder> httpFunctionBuilder) where TCommand : ICommand
        {
            string functionName = GetFunctionName<TCommand>(name);
            HttpFunctionDefinition definition = new HttpFunctionDefinition
            {
                Name = functionName
            };
            httpFunctionBuilder?.Invoke(new HttpFunctionBuilder(definition));
            return this;
        }

        public IFunctionBuilder StorageQueueFunction<TCommand>() where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionBuilder StorageQueueFunction<TCommand>(string functionName) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionBuilder StorageQueueFunction<TCommand>(Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionBuilder StorageQueueFunction<TCommand>(string functionName, Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        private static string GetFunctionName<TCommand>(string name) where TCommand : ICommand
        {
            string shortCommandName = typeof(TCommand).Name;
            if (name == null)
            {
                if (shortCommandName.EndsWith("Query"))
                {
                    name = shortCommandName.Substring(0, shortCommandName.Length - 5);
                }
                else if (shortCommandName.EndsWith("Command"))
                {
                    name = shortCommandName.Substring(0, shortCommandName.Length - 7);
                }
                else
                {
                    throw new ConfigurationException(
                        "A default Function name can only be used when a command types name ends with Query or Command");
                }
            }

            return name;
        }
    }
}
