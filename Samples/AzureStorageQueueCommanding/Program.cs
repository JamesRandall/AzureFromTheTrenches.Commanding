using System;
using System.Threading;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.AzureStorage;
using AccidentalFish.Commanding.Model;
using AccidentalFish.Commanding.Queue;
using AccidentalFish.DependencyResolver.MicrosoftNetStandard;
using AzureStorageQueueCommanding.Actors;
using AzureStorageQueueCommanding.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureStorageQueueCommanding
{
    class Program
    {
        private const string StorageAccountConnectionString =
                "YOUR-STORAGE-ACCOUNT-STRING";

        static void Main(string[] args)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
#pragma warning disable 4014
            RunDemo(cancellationTokenSource);            
#pragma warning restore 4014
            Console.ReadKey();
            cancellationTokenSource.Cancel();
        }

        private static async Task RunDemo(CancellationTokenSource cancellationTokenSource)
        {
            CloudQueue queue = await ConfigureQueue();
            ICommandDispatcher dispatcher;
            IAzureStorageCommandQueueProcessorFactory listenerFactory;
            ConfigureCommanding(queue, out dispatcher, out listenerFactory);

#pragma warning disable 4014 // we're just letting things run unmanaged in this console demo
            listenerFactory.Start<OutputToConsoleCommand, DeferredCommandResult>(queue, cancellationTokenSource.Token);
            dispatcher.DispatchAsync<OutputToConsoleCommand, DeferredCommandResult>(new OutputToConsoleCommand { Message = "Hello" });
#pragma warning restore 4014            
        }

        private static async Task<CloudQueue> ConfigureQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageAccountConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("outputtoconsolecommandqueue");
            await queue.CreateIfNotExistsAsync();
            return queue;
        }

        private static void ConfigureCommanding(CloudQueue queue, out ICommandDispatcher dispatcher, out IAzureStorageCommandQueueProcessorFactory listenerFactory)
        {
            MicrosoftNetStandardDependencyResolver resolver = new MicrosoftNetStandardDependencyResolver(new ServiceCollection());
            ICommandRegistry registry = resolver.UseCommanding(type => resolver.Register(type, type));
            resolver.UseCommandQueues().UseAzureStorageCommanding();

            ICommandDispatcher QueueDispatcher() => resolver.Resolve<IAzureStorageQueueDispatcherFactory>().Create(queue);
            registry.Register<OutputToConsoleCommand, OutputWorldToConsoleCommandActor>(dispatcherFactoryFunc: QueueDispatcher)
                .Register<OutputToConsoleCommand, OutputBigglesToConsoleCommandActor>();

            resolver.BuildServiceProvider();
            dispatcher = resolver.Resolve<ICommandDispatcher>();
            listenerFactory = resolver.Resolve<IAzureStorageCommandQueueProcessorFactory>();
        }
    }
}