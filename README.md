# AccidentalFish.Commanding

A simple configuration based asynchronous commanding framework that abstracts code away from how the command is dispatched and, ultimately, executed. Out the box support is provided for dispatch and execution to occur in-process, over HTTP, and in a deferred manner over Azure Storage Queues. Support is also provided for popping commands directly from queues and executing them and the new v2 includes support for auditing (and through that event sourcing).

The framework supports .NET Standard 1.4 (and higher) and so, at the time of writing, can be used with the following _minimum version_ runtimes:

* .NET 4.6.1
* .NET Core 1.0
* Mono 4.6
* Xamarin.iOS 10.0
* Xamarin.Android 10.0

As such it can happily be used in a varierty of scenarios such as ASP.Net, ASP.Net Core, Console apps, Worker Roles, WebJobs etc.

Additionally the framework is designed specifically for use with IoC containers and adapters are available for the below containers (the NuGet adapter package is in brackets):

* Autofac (AccidentalFish.DependencyResolver.Autofac)
* Ninject (AccidentalFish.DependencyResolver.Ninject)
* Unity (AccidentalFish.DependencyResolver.Unity)
* Microsoft ASP.NET Core Services Container (AccidentalFish.DependencyResolver.MicrosoftNetStandard)

_I've found the configuration based approach to be particularly useful when growing a project over time allowing for the building of a modular monolith early in the lifecycle (useful for exploring user interface, product design, market fit etc.) that can be easily pulled apart into an SOA or microservice architecture should the need arise. Microservices are great but if they're used too early and in the wrong context (for example while you have a handful of customers and are trying to establish market fit) can be as much of a hinderance in early stage development as they are a help when you need to scale. Note to self... finish blog post on evolutionary architecture!_

## Getting Started

Firstly install the nuget package:

    Install-Package AccidentalFish.Commanding

And install an adapter for your dependency resolver, for this documentation I'm going to assume the use of the Microsoft ASP.Net Core Services Container:

    Install-Package AccidentalFish.DependencyResolver.MicrosoftNetStandard

Although they don't have to be commands are best kept as just Plain Old Csharp Objects. They're best kept this way so that if you want to shift an in-memory command to a HTTP command it's super simple to do so. A command for adding two numbers together might look like this:

    public class AddCommand
    {
        public int FirstNumber { get; set; }

        public int SecondNumber { get; set; }
    }

The result of that command could be represented like this:

    public class MathResult
    {
        public int Value { get; set; }
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
    MicrosoftNetStandardDependencyResolver resolver = new MicrosoftNetStandardDependencyResolver(new ServiceCollection());
    resolver.UseCommanding(type => resolver.Register(type, type))
        .Register<AddCommand, AddCommandActor>();
    IServiceProvider serviceProvider = resolver.BuildServiceProvider();

The _UseCommanding_ method is an extension method on the dependency resolver that registers the injectable commaning interfaces and returns an _ICommandRegistry_ interface that allows you to register commands and their actors. _(For the curious the lambda that is supplied to it registers actors as concrete types in the IoC container - this is only required for IoC containers that can't resolver arbitary concrete types)_

To dispatch our command we need to get hold of the ICommandDispatcher interface and send the command. We'll do that and output the result to the console:

    ICommandDispatcher commandDispatcher = serviceProvider.GetService<ICommandDispatcher>();
    MathResult mathResult = await commandDispatcher.DispatchAsync<AddCommand, MathResult>(new AddCommand { FirstNumber = 5, SecondNumber = 6});
    Console.WriteLine(mathResult.Value); // hopefully says 11

If you don't care about a result you can shorten this a little:

    await commandDispatcher.DispatchAsync(new AddCommand { FirstNumber = 5, SecondNumber = 6});

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
