using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Tests.Performance.Console.Model;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.Tests.Performance.Console
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private const int CommandsToExecute = 10000000;
        static void Main(string[] args)
        {
            ConsoleKeyInfo keyInfo;
            do
            {
                System.Console.Clear();
                System.Console.WriteLine($"1. Dispatch {CommandsToExecute} commands with results");
                System.Console.WriteLine($"2. Dispatch {CommandsToExecute} commands with no result");
                System.Console.WriteLine($"3. Dispatch {CommandsToExecute} commands with no result excluding compile time");
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
                        ExecuteCommandsWithNoResultsExcludeCompileTime();
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
            CommandingDependencyResolver dependencyResolver = serviceCollection.GetCommandingDependencyResolver(() => _serviceProvider);
            Options options = new Options
            {
                Reset = true // we reset the registry because we allow repeat runs, in a normal app this isn't required                
            };
            dependencyResolver.UseCommanding(options)
                .Register<SimpleActor>();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            return _serviceProvider.GetService<ICommandDispatcher>();
        }

        public static async Task ExecuteCommandsWithResults()
        {
            ICommandDispatcher dispatcher = Configure();
            SimpleCommand command = new SimpleCommand();
            Stopwatch sw = Stopwatch.StartNew();
            for (int index = 0; index < CommandsToExecute; index++)
            {
                SimpleResult result = await dispatcher.DispatchAsync(command);
            }
            sw.Stop();
            System.Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");
            System.Console.WriteLine($"Took {(double)sw.ElapsedMilliseconds / (double)CommandsToExecute}ms on average per command");
        }

        public static async Task ExecuteCommandsWithNoResults()
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
        }

        public static async Task ExecuteCommandsWithNoResultsExcludeCompileTime()
        {
            ICommandDispatcher dispatcher = Configure();
            SimpleCommand command = new SimpleCommand();
            await dispatcher.DispatchAsync(command); // force a compile before running the stopwatch
            Stopwatch sw = Stopwatch.StartNew();
            for (int index = 0; index < CommandsToExecute; index++)
            {
                await dispatcher.DispatchAsync(command);
            }
            sw.Stop();
            System.Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");
        }
    }
}