using System;
using System.Threading;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.AzureStorage;
using AccidentalFish.Commanding.Model;
using AccidentalFish.DependencyResolver.MicrosoftNetStandard;
using AzureStorageAuditing.Actors;
using AzureStorageAuditing.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorageAuditing
{
    class Program
    {
       // private const string StorageAccountConnectionString =
       //     "YOUR-STORAGE-ACCOUNT-STRING";

        static void Main(string[] args)
        {
#pragma warning disable 4014
            RunDemo();
#pragma warning restore 4014
            Console.ReadKey();
        }

        private static async Task RunDemo()
        {
            ICommandDispatcher dispatcher = await Configure();
            await dispatcher.DispatchAsync(new ChainCommand());
        }

        private static async Task<ICommandDispatcher> Configure()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("commandauditpayload");
            CloudTable byDate = tableClient.GetTableReference("commandauditbydate");
            CloudTable byCorrelationId = tableClient.GetTableReference("commandauditbycorrelationid");
            await blobContainer.CreateIfNotExistsAsync();
            await byDate.CreateIfNotExistsAsync();
            await byCorrelationId.CreateIfNotExistsAsync();

            MicrosoftNetStandardDependencyResolver resolver = new MicrosoftNetStandardDependencyResolver(new ServiceCollection());
            Options options = new Options
            {
                CommandActorContainerRegistration = type => resolver.Register(type, type),
                Reset = true // we reset the registry because we allow repeat runs, in a normal app this isn't required                
            };
            resolver.UseCommanding(options)
                .Register<ChainCommand, ChainCommandActor>()
                .Register<OutputToConsoleCommand, OutputWorldToConsoleCommandActor>();
            resolver.UseAzureStorageCommandAuditing(byCorrelationId, byDate, blobContainer);
            resolver.BuildServiceProvider();
            return resolver.Resolve<ICommandDispatcher>();
        }
    }
}