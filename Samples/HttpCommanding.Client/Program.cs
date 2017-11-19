using System;
using System.Net.Http;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Http;
using HttpCommanding.Model.Commands;
using HttpCommanding.Model.Results;
using InMemoryCommanding;
using Microsoft.Extensions.DependencyInjection;

namespace HttpCommanding.Client
{
    class Program
    {
        private static IServiceProvider _serviceProvider = null;

        static void Main(string[] args)
        {
            ICommandDispatcher dispatcher = Configure();
            Console.WriteLine("Press a key to execute HTTP command");
            Console.ReadKey();
#pragma warning disable 4014
            ExecuteHttpCommand(dispatcher);
#pragma warning restore 4014
            Console.ReadKey();            
        }

        static ICommandDispatcher Configure()
        {
            Uri uri = new Uri("http://localhost:52933/api/personalDetails");
            ServiceCollection serviceCollection = new ServiceCollection();
            CommandingDependencyResolver dependencyResolver = serviceCollection.GetCommandingDependencyResolver(() => _serviceProvider);
            Options options = new Options
            {
                CommandActorContainerRegistration = type => serviceCollection.AddTransient(type, type),
            };

            ICommandRegistry registry = CommandingDependencies.UseCommanding(dependencyResolver, options);
            HttpCommandingDependencies.UseHttpCommanding(dependencyResolver);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            IHttpCommandDispatcherFactory httpCommandDispatcherFactory = _serviceProvider.GetService<IHttpCommandDispatcherFactory>();
            registry.Register<UpdatePersonalDetailsCommand, UpdateResult>(() => httpCommandDispatcherFactory.Create(uri, HttpMethod.Put));

            ICommandDispatcher dispatcher = _serviceProvider.GetService<ICommandDispatcher>();
            return dispatcher;
        }

        static async Task ExecuteHttpCommand(ICommandDispatcher dispatcher)
        {            
            UpdateResult result = await dispatcher.DispatchAsync(
                new UpdatePersonalDetailsCommand
                {
                    Age = 10,
                    Forename = "Jim",
                    Surname = "McCoy",
                    Id = Guid.NewGuid()
                });
            Console.WriteLine(result.DidUpdate);
            Console.WriteLine(result.ValidationMessage);
        }
    }
}