using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Builders
{
    internal class FunctionBuilder : IFunctionBuilder
    {
        private readonly List<AbstractFunctionDefinition> _definitions = new List<AbstractFunctionDefinition>();
        private readonly HttpFunctionBuilder _defaultBuilder;

        public FunctionBuilder()
        {
            _defaultBuilder = new HttpFunctionBuilder(string.Empty, _definitions);
        }

        public IFunctionBuilder HttpRoute(string routePrefix, Action<IHttpFunctionBuilder> httpRouteBuilder)
        {
            HttpFunctionBuilder builder = new HttpFunctionBuilder(routePrefix, _definitions);
            httpRouteBuilder(builder);
            return this;
        }

        public IFunctionBuilder HttpFunction<TCommand>() where TCommand : ICommand
        {
            _defaultBuilder.HttpFunction<TCommand>();
            return this;
        }

        public IFunctionBuilder HttpFunction<TCommand>(HttpMethod method) where TCommand : ICommand
        {
            _defaultBuilder.HttpFunction<TCommand>(method);
            return this;
        }

        public IFunctionBuilder HttpFunction<TCommand>(string route, HttpMethod method) where TCommand : ICommand
        {
            _defaultBuilder.HttpFunction<TCommand>(route, method);
            return this;
        }

        public IFunctionBuilder HttpFunction<TCommand>(Action<IHttpFunctionConfiguration> httpFunctionBuilder) where TCommand : ICommand
        {
            _defaultBuilder.HttpFunction<TCommand>(httpFunctionBuilder);
            return this;
        }

        public IFunctionBuilder HttpFunction<TCommand>(string route, Action<IHttpFunctionConfiguration> httpFunctionBuilder) where TCommand : ICommand
        {
            _defaultBuilder.HttpFunction<TCommand>(route, httpFunctionBuilder);
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
        
        public IReadOnlyCollection<HttpFunctionDefinition> GetHttpFunctionDefinitions()
        {
            return _definitions.OfType<HttpFunctionDefinition>().ToArray();
        }

        public IReadOnlyCollection<AbstractFunctionDefinition> Definitions => _definitions;        
    }
}
