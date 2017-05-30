using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.AzureStorage;
using AccidentalFish.Commanding.AzureStorage.Strategies;
using AccidentalFish.DependencyResolver.MicrosoftNetStandard;
using AzureStorageAuditing.Actors;
using AzureStorageAuditing.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;

namespace AzureStorageAuditing
{
    class Program
    {
       // private const string StorageAccountConnectionString =
       //     "YOUR-STORAGE-ACCOUNT-STRING";

        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("1. Use single table storage strategy");
                Console.WriteLine("2. Use per day table storage strategy");
                ConsoleKeyInfo info = Console.ReadKey();

                IStorageStrategy storageStrategy = info.Key == ConsoleKey.D1 ? (IStorageStrategy)new SingleTableStrategy() : new TablePerDayStrategy();

#pragma warning disable 4014
                RunDemo(storageStrategy);
#pragma warning restore 4014
                Console.ReadKey();
            } while (true);            
        }

        private static async Task RunDemo(IStorageStrategy storageStrategy)
        {
            ICommandDispatcher dispatcher = Configure(storageStrategy);
            await dispatcher.DispatchAsync(new ChainCommand());
        }

        private static int _counter = -1;

        private static ICommandDispatcher Configure(IStorageStrategy storageStrategy)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            
            MicrosoftNetStandardDependencyResolver resolver = new MicrosoftNetStandardDependencyResolver(new ServiceCollection());
            IReadOnlyDictionary<string, object> Enricher(IReadOnlyDictionary<string, object> existing) => new Dictionary<string, object> { { "ExampleEnrichedCounter", Interlocked.Increment(ref _counter) } };
            Options options = new Options
            {
                CommandActorContainerRegistration = type => resolver.Register(type, type),
                Reset = true, // we reset the registry because we allow repeat runs, in a normal app this isn't required                
                Enrichers = new [] { new FunctionWrapperCommandDispatchContextEnricher(Enricher) }
            };            

            resolver.UseCommanding(options)
                .Register<ChainCommand, ChainCommandActor>()
                .Register<OutputToConsoleCommand, OutputWorldToConsoleCommandActor>();
            resolver.UseAzureStorageCommandAuditing(storageAccount, storageStrategy: storageStrategy);
            resolver.BuildServiceProvider();
            return resolver.Resolve<ICommandDispatcher>();
        }
    }
}