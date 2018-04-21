using System;
using AzureFromTheTrenches.Commanding.AzureFunctions;
using Microsoft.Extensions.DependencyInjection;
using SampleFunctionConfiguration.Commands;
using SampleFunctionConfiguration.Services;
using SampleFunctionConfiguration.Services.Implementation;

namespace SampleFunctionConfiguration
{
    public class MyFunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                // register services
                .Services(services => services.AddTransient<ICalculator, Calculator>())
                // register command handlers
                .Register(registry => registry.Discover<MyFunctionAppConfiguration>()) // use the command and handler auto-discovery process
                // register functions - by default the functions will be given the name of the command minus the postfix Command and use the GET verb
                .Functions(functions => functions
                    .HttpFunction<EchoMessageCommand>()
                    .HttpFunction<AddCommand>()
                    .StorageQueueFunction<BackgroundOperationCommand>());
        }
    }
}



