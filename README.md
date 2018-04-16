# AzureFromTheTrenches.Commanding
![Build Status](https://accidentalfish.visualstudio.com/_apis/public/build/definitions/09076561-bff4-4f58-b28a-ff4b483b7e65/21/badge)

Note that [full documentation](https://commanding.azurefromthetrenches.com) including [guides](https://commanding.azurefromthetrenches.com/guides/introduction.html) and an [API reference](https://commanding.azurefromthetrenches.com/api/index.html) can be found on the help site.

I also have a series on moving from "onion layer" architecture to making use of a mediated command pattern in a [series of posts on my blog](https://www.azurefromthetrenches.com/c-cloud-application-architecture-commanding-with-a-mediator-the-full-series/).

## Introduction

AzureFromTheTrenches.Commanding is a configuration based asynchronous command mediator framework with a number of key design goals:

* To provide a highly performant mediator for simple usage
* To support evolution across a projects lifecycle allowing for easy decomposition from a modular-monolith to a fully distributed micro-service architecture
* To provide a none-leaking abstraction over command dispatch and execution semantics
* To reduce boilerplate code - simplistically less code means less errors and less to maintain

To support these goals the framework supports .NET Standard 2.0 (and higher) and so can be used in a wide variety of scenarios and a number of fully optional extension packages are available to enable:

* Building a [REST API](restApi/quickstart.md) directly from commands using a configuration based approach
* Dispatching commands to queues (Service Bus Queues and Topics, and Azure Storage)
* Dispatching commands to event hubs
* Using queues as a source for executing commands 
* Caching commands based on signatures in local memory caches or Redis caches

You don't need to take advantage of that functionality but you can, if you want, adopt it over time without changing your core code.

## Getting Started

Firstly install the nuget package for commanding:

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

Commands are acted on by handlers and our add handler looks like this:

    public AddCommandHandler : ICommandHandler<AddCommand, MathResult>
    {
        public Task<MathResult> ExecuteAsync(AddCommand command, MathResult previousResult)
        {
            return new MathResult {
                Value = command.FirstNumber + command.SecondNumber
            };
        }
    }

Having defined our command, result and handler, we need to register these with the commanding system. If you're just writing a console app you can do this in Program.cs but for more realistic usage you'd do this where you configure your IoC container - it's handy to think of command registrations as just another part of your applications configuration, besides which you'll need access to the container. The example below demonstrates registration with the Microsoft ASP.Net Core Service Provider:

    // First register the commanding framework with the IoC container
    IServiceProvider serviceProvider = null;
    IServiceCollection serviceCollection = new ServiceCollection();
    CommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
        (fromType, toInstance) => services.AddSingleton(fromType, toInstance),
        (fromType, toType) => services.AddTransient(fromType, toType),
        (resolveTo) => _serviceProvider.GetService(resolveTo));
    ICommandRegistry registry = adapter.AddCommanding();
    serviceProvider = serviceCollection.BuildServiceProvider();

    // Now register our handler
    registry.Register<AddCommandHandler>();

The _CommandingDependencyResolverAdapter_ class is an adapter that allows the framework to be registed with any IoC container and the _AddCommanding_ method registers the injectable commaning interfaces and returns an _ICommandRegistry_ interface that allows you to register handlers - you only need to register a handler, the framework will figure out the rest and registration uses a fluent API style for concise readable code.

To dispatch our command we need to get hold of the ICommandDispatcher interface and send the command. We'll do that and output the result to the console:

    ICommandDispatcher commandDispatcher = dependencyResolver.ServiceProvider.GetService<ICommandDispatcher>();
    MathResult mathResult = await commandDispatcher.DispatchAsync(new AddCommand { FirstNumber = 5, SecondNumber = 6});
    Console.WriteLine(mathResult.Value); // hopefully says 11

And for simple usage that's it. This example is a bit contrived as we're resolving dependencies by hand and theres a lot of boilerplate to add two numbers together but in real world scenarios all you really need to do is register your command handlers in the appropriate place, for example if you're using ASP.Net Core then all the dependency injection boilerplate is in place.

## Samples

1. Simple in memory command execution
<https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/InMemoryCommanding>

2. Dispatching commands over HTTP
<https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/HttpCommanding.Client>

3. Executing commands in response to HTTP requests (ASP.Net Core)
<https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/HttpCommanding.Web>

4. Dispatching to and executing from Azure Storage queues
<https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/AzureStorageQueueCommanding>

5. Azure storage auditing
<https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/AzureStorageAuditing>

6. Azure event hub auditing
<https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/AzureEventHubAuditing>

## Advanced Usage

Further usage scenarios can be found in the [wiki](https://github.com/JamesRandall/AccidentalFish.Commanding/wiki) including:

* [Configuration Options](https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/wiki/9.-Configuration-Options)
* [HTTP Execution and Dispatch](https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/wiki/7.-HTTP-Dispatch-and-Execution)
* [Azure Queue Dispatch and Execution](https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/wiki/8.-Azure-Queue-Dispatch-and-Execution)
* [Auditing and Event Sourcing](https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/wiki/6.-Auditing-and-Event-Sourcing)

## Support

If you get stuck log a GitHub issue or catch me over on Twitter at [@azuretrenches](https://twitter.com/azuretrenches) and I'll help if I can.
