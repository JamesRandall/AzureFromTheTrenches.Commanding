using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Builders
{
    class HttpFunctionBuilder : IHttpFunctionBuilder
    {
        private static readonly HttpMethod DefaultMethod = HttpMethod.Get;
        private readonly string _routePrefix;
        private readonly List<AbstractFunctionDefinition> _definitions;

        public HttpFunctionBuilder(string routePrefix, List<AbstractFunctionDefinition> definitions)
        {
            _routePrefix = routePrefix;
            _definitions = definitions;
        }

        public IHttpFunctionBuilder HttpFunction<TCommand>() where TCommand : ICommand
        {
            return HttpFunction<TCommand>(null, DefaultMethod);
        }

        public IHttpFunctionBuilder HttpFunction<TCommand>(HttpMethod method) where TCommand : ICommand
        {
            return HttpFunction<TCommand>(null, method);
        }

        public IHttpFunctionBuilder HttpFunction<TCommand>(string route, HttpMethod method) where TCommand : ICommand
        {
            return HttpFunction<TCommand>(route, builder => builder.AddVerb(method));
        }

        public IHttpFunctionBuilder HttpFunction<TCommand>(string route) where TCommand : ICommand
        {
            return HttpFunction<TCommand>(route, DefaultMethod);
        }

        public IHttpFunctionBuilder HttpFunction<TCommand>(Action<IHttpFunctionConfiguration> httpFunctionBuilder) where TCommand : ICommand
        {
            return HttpFunction<TCommand>(null, httpFunctionBuilder);
        }

        public IHttpFunctionBuilder HttpFunction<TCommand>(string route, Action<IHttpFunctionConfiguration> httpFunctionBuilder) where TCommand : ICommand
        {
            string functionName = GetFunctionName<TCommand>();
            HttpFunctionDefinition definition = new HttpFunctionDefinition
            {
                Name = functionName,
                CommandType = typeof(TCommand),
                Route = string.Concat(_routePrefix, route)
            };
            httpFunctionBuilder?.Invoke(new HttpFunctionConfiguration(definition));
            _definitions.Add(definition);
            return this;
        }

        private static string GetFunctionName<TCommand>() where TCommand : ICommand
        {
            string shortCommandName = typeof(TCommand).Name;

            if (shortCommandName.EndsWith("Query"))
            {
                return shortCommandName.Substring(0, shortCommandName.Length - 5);
            }
            if (shortCommandName.EndsWith("Command"))
            {
                return shortCommandName.Substring(0, shortCommandName.Length - 7);
            }

            return shortCommandName;            
        }
    }
}
