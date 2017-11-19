# AzureFromTheTrenches.Commanding

A simple configuration based asynchronous commanding framework designed to be both easy to use and highly extensible allowing projects
to start with a simple in memory based approach to commanding and over time adopt advanced techniques such as event sourcing, remote commanding
over REST and auditing. Out the box support is provided for dispatch and execution to occur in-process, over HTTP, and in a deferred manner over Azure Storage Queues. Support is also provided for popping commands directly from queues and executing them along with support for auditing.

The framework supports .NET Standard 2.0 (and higher) and so, at the time of writing, can be used with the following _minimum version_ runtimes:

* .NET 4.6.1
* .NET Core 2.0
* Mono 5.4
* Xamarin.iOS 10.14
* Xamarin.Mac 3.8
* Xamarin.Android 10.8
* Universal Windows Platform 10.0.16299

As such it can easily be used in a varierty of scenarios such as ASP.Net, ASP.Net Core, Console apps, Worker Roles, WebJobs etc.

Additionally the framework is designed specifically for use with a dependency injection approach and can be used with any of the
common dependency injectors (e.g. Autofac, Ninject, Unity, ASP.Net Core Services Container etc.) through the use of a simple adapter.

## Getting Started

Firstly install the nuget package:

    Install-Package AzureFromTheTrenches.Commanding

As an example let's create a command that adds two numbers together and returns a result:

    public class MathResult
    {
        public int Value { get; set; }
    }
    
    public class AddCommand : ICommand<MathResult>
    {
        public int FirstNumber { get; set; }

        public int SecondNumber { get; set; }
    }

Commands are acted on by actors and our add action looks like this:

    public AddCommandActor : ICommandActor<AddCommand, MathResult>
    {
        public Task<MathResult> ExecuteAsync(AddCommand command, MathResult previousResult)
        {
            return new MathResult {
                Value = command.FirstNumber + command.SecondNumber
            };
        }
    }

Having defined our command, result and actor, we need to register these with the commanding system. If you're just writing a console app you can do this in Program.cs but for more realistic usage you'd do this where you configure your IoC container - it's handy to think of command registrations as just another part of your applications configuration, besides which you'll need access to the container. The example below demonstrates registration with the Microsoft ASP.Net Core Service Provider:

    IServiceCollection serviceCollection = new ServiceCollection();
    IServiceProvider serviceProvider = null;
    ICommandingDependencyResolver commandingDependencyResolver = new CommandingDependencyResolver(
        (type, instance) => serviceCollection.AddSingleton(type, instance),
        (type, impl) => serviceCollection.AddTransient(type, impl),
        type => serviceProvider.GetService(type)
    );
    commandingDependencyResolver.UseCommanding()
        .Register<AddCommandActor>();
    IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

The _UseCommanding_ method is an extension method on the dependency resolver that registers the injectable commaning interfaces and returns an _ICommandRegistry_ interface that allows you to register commands and their actors.

To dispatch our command we need to get hold of the ICommandDispatcher interface and send the command. We'll do that and output the result to the console:

    ICommandDispatcher commandDispatcher = serviceProvider.GetService<ICommandDispatcher>();
    MathResult mathResult = await commandDispatcher.DispatchAsync(new AddCommand { FirstNumber = 5, SecondNumber = 6});
    Console.WriteLine(mathResult.Value); // hopefully says 11

And for simple usage that's it. The above is a bit contrived as we're resolving dependencies by hand and theres a lot of boilerplate to add two numbers together but in real world scenarios all you really need to do is register your commands in the appropriate place.

Take a look at the samples and then more advanced usage is outlined further down.

## Samples

1. Simple in memory command execution
<https://github.com/JamesRandall/AccidentalFish.Commanding/tree/master/Samples/InMemoryCommanding>

2. Dispatching commands over HTTP
<https://github.com/JamesRandall/AccidentalFish.Commanding/tree/master/Samples/HttpCommanding.Client>

3. Executing commands in response to HTTP requests (ASP.Net Core)
<https://github.com/JamesRandall/AccidentalFish.Commanding/tree/master/Samples/HttpCommanding.Web>

4. Dispatching to and executing from Azure Storage queues
<https://github.com/JamesRandall/AccidentalFish.Commanding/tree/master/Samples/AzureStorageQueueCommanding>

5. Azure storage auditing
<https://github.com/JamesRandall/AccidentalFish.Commanding/tree/master/Samples/AzureStorageAuditing>

## Advanced Usage

Further usage scenarios can be found in the [wiki](https://github.com/JamesRandall/AccidentalFish.Commanding/wiki) including:

* [Configuration Options](https://github.com/JamesRandall/AccidentalFish.Commanding/wiki/Configuration-Options)
* [HTTP Execution and Dispatch](https://github.com/JamesRandall/AccidentalFish.Commanding/wiki/HTTP-Dispatch-and-Execution)
* [Azure Queue Dispatch and Execution](https://github.com/JamesRandall/AccidentalFish.Commanding/wiki/Azure-Queue-Dispatch-and-Execution)
* [Auditing and Event Sourcing](https://github.com/JamesRandall/AccidentalFish.Commanding/wiki/Auditing-and-Event-Sourcing)

## Support

If you get stuck log a GitHub issue or catch me over on Twitter at [@azuretrenches](https://twitter.com/azuretrenches) and I'll help if I can.
