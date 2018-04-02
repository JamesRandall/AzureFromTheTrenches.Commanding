using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Tests.Performance.Console.Model;
using AzureFromTheTrenches.Commanding.Tests.Performance.Console.Model.Mediatr;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.Tests.Performance.Console
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private const int CommandsToExecute = 10000000;
        private const int ParallelTasks = 4;
        //private const int CommandsToExecute = 1000000;
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                ExecuteCommandsWithResults().Wait();
                return;
            }
            ConsoleKeyInfo keyInfo;
            do
            {
                System.Console.Clear();
                System.Console.WriteLine($"1. Dispatch {CommandsToExecute} commands with results");
                System.Console.WriteLine($"2. Dispatch {CommandsToExecute} commands with no result");
                System.Console.WriteLine($"3. Dispatch {CommandsToExecute} commands with results through Mediatr");
                System.Console.WriteLine($"4. Dispatch {CommandsToExecute} commands with results over {ParallelTasks} tasks");
                System.Console.WriteLine($"5. Dispatch {CommandsToExecute} commands with results over {ParallelTasks} tasks with Mediatr");
                System.Console.WriteLine($"6. Execute {CommandsToExecute} calls on a class with results");
                System.Console.WriteLine("");
                System.Console.WriteLine("Esc - quit");
                keyInfo = System.Console.ReadKey();
                System.Console.WriteLine();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
#pragma warning disable 4014
                        ExecuteCommandsWithResults();
#pragma warning restore 4014
                        break;
                    case ConsoleKey.D2:
#pragma warning disable 4014
                        ExecuteCommandsWithNoResults();
#pragma warning restore 4014
                        break;
                    case ConsoleKey.D3:
#pragma warning disable 4014
                        ExecuteCommandsWithMediatr();
#pragma warning restore 4014
                        break;
                    case ConsoleKey.D4:
#pragma warning disable 4014
                        ExecuteParallelCommandsWithResults();
#pragma warning restore 4014
                        break;
                    case ConsoleKey.D5:
#pragma warning disable 4014
                        ExecuteParallelCommandsWithMediatr();
#pragma warning restore 4014
                        break;
                    case ConsoleKey.D6:
#pragma warning disable 4014
                        ExecuteDirectlyOnAClass();
#pragma warning restore 4014
                        break;
                }
                if (keyInfo.Key != ConsoleKey.Escape)
                {
                    System.Console.ReadKey();
                }
            } while (keyInfo.Key != ConsoleKey.Escape);
        }

        private static ICommandDispatcher Configure()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            CommandingDependencyResolverAdapter dependencyResolver = serviceCollection.GetCommandingDependencyResolver(() => _serviceProvider);
            Options options = new Options
            {
                DisableCorrelationIds = true, // as a comparison to mediatr we disable correlation IDs
                Reset = true // we reset the registry because we allow repeat runs, in a normal app this isn't required                
            };
            dependencyResolver.AddCommanding(options)
                .Register<SimpleHandler>();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            return _serviceProvider.GetService<ICommandDispatcher>();
        }

        private static ICommandDispatcher ConfigureNoResult()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            CommandingDependencyResolverAdapter dependencyResolver = serviceCollection.GetCommandingDependencyResolver(() => _serviceProvider);
            Options options = new Options
            {
                DisableCorrelationIds = true, // as a comparison to mediatr we disable correlation IDs
                Reset = true // we reset the registry because we allow repeat runs, in a normal app this isn't required                
            };
            dependencyResolver.AddCommanding(options)
                .Register<SimpleHandlerNoResult>();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            return _serviceProvider.GetService<ICommandDispatcher>();
        }

        public static async Task ExecuteDirectlyOnAClass()
        {
            SimpleClass actingClass = new SimpleClass();
            Stopwatch sw = Stopwatch.StartNew();
            for (int index = 0; index < CommandsToExecute; index++)
            {
                await actingClass.DoSomething();
            }
            sw.Stop();
            System.Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");
            System.Console.WriteLine($"Took {(double)sw.ElapsedMilliseconds / (double)CommandsToExecute}ms on average per call");
        }

        public static async Task ExecuteCommandsWithResults()
        {
            ICommandDispatcher dispatcher = Configure();
            SimpleCommand command = new SimpleCommand();
            Stopwatch sw = Stopwatch.StartNew();
            for (int index = 0; index < CommandsToExecute; index++)
            {
                await dispatcher.DispatchAsync(command);
            }
            sw.Stop();
            System.Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");
            System.Console.WriteLine($"Took {(double)sw.ElapsedMilliseconds / (double)CommandsToExecute}ms on average per command");
        }

        public static async Task ExecuteParallelCommandsWithResults()
        {
            ICommandDispatcher dispatcher = Configure();
            SimpleCommand command = new SimpleCommand();
            int perTaskCommands = CommandsToExecute / ParallelTasks;
            Task[] tasks = new Task[ParallelTasks];
            Stopwatch sw = Stopwatch.StartNew();
            for (int taskIndex = 0; taskIndex < ParallelTasks; taskIndex++)
            {
                tasks[taskIndex] = Task.Run(async () =>
                {
                    for (int index = 0; index < perTaskCommands; index++)
                    {
                        await dispatcher.DispatchAsync(command);
                    }
                });
            }

            await Task.WhenAll(tasks);
            sw.Stop();
            System.Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");
            System.Console.WriteLine($"Took {(double)sw.ElapsedMilliseconds / (double)CommandsToExecute}ms on average per command");
        }

        public static async Task ExecuteCommandsWithNoResults()
        {
            ICommandDispatcher dispatcher = ConfigureNoResult();
            SimpleCommandNoResult command = new SimpleCommandNoResult();
            Stopwatch sw = Stopwatch.StartNew();
            for (int index = 0; index < CommandsToExecute; index++)
            {
                await dispatcher.DispatchAsync(command);
            }
            sw.Stop();
            System.Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");
        }

        public static async Task ExecuteCommandsWithMediatr()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddMediatR();
            serviceCollection.AddTransient<IRequestHandler<SimpleMediatrRequest, SimpleResult>, SimpleMediatrRequestHandler>();
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            IMediator mediator = serviceProvider.GetService<IMediator>();
            SimpleMediatrRequest request = new SimpleMediatrRequest();
            Stopwatch sw = Stopwatch.StartNew();
            for (int index = 0; index < CommandsToExecute; index++)
            {
                await mediator.Send(request);
            }
            sw.Stop();
            System.Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");
            System.Console.WriteLine($"Took {(double)sw.ElapsedMilliseconds / (double)CommandsToExecute}ms on average per command");
        }

        public static async Task ExecuteParallelCommandsWithMediatr()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddMediatR();
            serviceCollection.AddTransient<IRequestHandler<SimpleMediatrRequest, SimpleResult>, SimpleMediatrRequestHandler>();
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            IMediator mediator = serviceProvider.GetService<IMediator>();
            SimpleMediatrRequest request = new SimpleMediatrRequest();
            int perTaskCommands = CommandsToExecute / ParallelTasks;
            Task[] tasks = new Task[ParallelTasks];
            Stopwatch sw = Stopwatch.StartNew();
            for (int taskIndex = 0; taskIndex < ParallelTasks; taskIndex++)
            {
                tasks[taskIndex] = Task.Run(async () =>
                {
                    for (int index = 0; index < perTaskCommands; index++)
                    {
                        await mediator.Send(request);
                    }
                });
            }

            await Task.WhenAll(tasks);
            sw.Stop();
            System.Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");
            System.Console.WriteLine($"Took {(double)sw.ElapsedMilliseconds / (double)CommandsToExecute}ms on average per command");
        }
    }
}