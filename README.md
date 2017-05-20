# AccidentalFish.Commanding

A simple configuration based asynchronous commanding framework that abstracts code away from how the command is dispatched and, ultimately, executed. Out the box support is provided for dispatch and execution to occur in-process, over HTTP, and in a deferred manner over Azure Storage Queues. Support is also provided for popping commands directly from queues and executing them.

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

## HTTP Dispatch and Execution

To understand this it's best to look at the [client sample](https://github.com/JamesRandall/AccidentalFish.Commanding/tree/master/Samples/HttpCommanding.Client) and the [server sample](https://github.com/JamesRandall/AccidentalFish.Commanding/tree/master/Samples/HttpCommanding.Web) however I'll cover getting started with this here. First you'll need to add the NuGet package:

    Install-Package AccidentalFish.Commanding.Http

Registration of a command is, optionally, slightly different if you want to use HTTP as you don't need to register an actor and you'll also need to install the HTTP handlers:

    resolver.UseHttpCommanding();
    resolver.BuildServiceProvider(); // only required if you are using a IoC container that requires a build step
    IHttpCommandDispatcherFactory httpCommandDispatcherFactory = resolver.Resolve<IHttpCommandDispatcherFactory>();
    registry.Register<AddCommand>(() => httpCommandDispatcherFactory.Create(uri, HttpMethod.Put));

There's nothing stopping you registering an actor too - but it won't do anything as the provided dispatcher will simply not execute actors locally (though it is possible to write a dispatcher that does execute locally and dispatch remotely). Note also above that you can specify both URI and HTTP verb for the command.

Dispatching the command is exactly the same as for in process immediate execution (other than that it will take a little longer):

    MathResult mathResult = await commandDispatcher.DispatchAsync<AddCommand, MathResult>(new AddCommand { FirstNumber = 5, SecondNumber = 6});
    Console.WriteLine(mathResult.Value);

Hopefully the above illustrates how you can change how commands are executed without changing your code, instead like with an IoC container, just the configuration.

To execute the command in the HTTP server (and I'm assuming you're using Web API or ASP.Net Core MVC in the below) then you need to inject an instance of _ICommandExecuter_ into your controller and execute the command (you could also use the _ICommandDispatcher_ interface if you wish to take advantage of its broader functionality). In ASP.Net Core MVC this looks like the below:

    [Route("api/[controller]")]
    public class AddController : Controller
    {
        private readonly ICommandExecuter _commandExecuter;

        public AddController(ICommandExecuter commandExecuter)
        {
            _commandExecuter = commandExecuter;
        }

        [HttpPut]
        public async Task<MathResult> Put([FromBody]AddCommand command)
        {
            UpdateResult result = await _commandExecuter.ExecuteAsync<AddCommand, MathResult>(command);
            return result;
        }        
    }

There's nothing special about the configuration on the server side. In fact it looks exactly like our in memory example from earlier but simply sits in the _ConfigureServices_ method of the ASP.Net Core startup class:

    public void ConfigureServices(IServiceCollection services)
    {
        // Add framework services.
        services.AddMvc();
        MicrosoftNetStandardDependencyResolver resolver = new MicrosoftNetStandardDependencyResolver(services);
        resolver.UseCommanding(type => resolver.Register(type, type))
            .Register<UpdatePersonalDetailsCommand, UpdatePersonalDetailsCommandActor>();
        resolver.BuildServiceProvider();
    }

By default the built in serializer use JSON however you can supply an alternative serializer by implementing the IHttpCommandSerializer interface and registering it when you install HTTP commanding, for example:

    resolver.UseHttpCommanding<MyCustomSerializer>();

The IHttpCommandSerializer interface defines methods for serializing, deserializing and stating the MIME / media type.

_Note: you'd be correct in assuming I've got some additional functionality coming that allows you to set up the REST API for a command automatically without the above boilerplate controller and action handler_

## Azure Queue Dispatch and Execution
_(Note: Service bus equivelants will be available as soon as the Microsoft Service Bus package for .NET Standard exits preview)_

A [sample is available](https://github.com/JamesRandall/AccidentalFish.Commanding/tree/master/Samples/AzureStorageQueueCommanding) that illustrates this however I'll also cover the important points below. I'm going to assume a couple of things in the below:

1. You have a queue of type CloudQueue called myQueue:
    CloudQueue myQueue = .... queue from CloudQueueClient ....;
2. That your dispatch and reciept are in different systems (for example a REST API dispatches the command and a Web Job processes it)

To get started you'll need to install a couple of NuGet packages in both the dispatch and process halves (so in our example in the REST API and the Web Job):

    Install-Package AccidentalFish.Commanding.Queue
    Install-Package AccidentalFish.Commanding.AzureStorage

And then install the queue based commanding system:

    resolver.UseCommandQueues().UseAzureStorageCommanding();

On the dispatch side registering (our REST API) a command is a little more complicated with a queue as you need to supply the Azure Storage Queue dispatcher as a custom dispatcher:

    ICommandDispatcher QueueDispatcher() => resolver.Resolve<IAzureStorageQueueDispatcherFactory>().Create(queue);
    registry.Register<AddCommand>(QueueDispatcher);

You'll note in the above I don't specify a result type. Queue dispatch is currently one way only. Dispatching a command is the same as ever:

    await commandDispatcher.DispatchAsync(new AddCommand { FirstNumber = 5, SecondNumber = 6});

On the queue processing side (our Web Job) again you need to install the queue based commanding system and register our command with it's actor.

    ICommandRegistry registry = resolver.UseCommanding(type => resolver.Register(type, type));
    resolver.UseCommandQueues().UseAzureStorageCommanding();
    registry.Register<AddCommand, AddCommandActor>();

Although you could write code to pull items from the queue and execute them much as we did with HTTP Helper systems are provided that will watch a queue (with a backoff algorithm) and dispatch the items that appear for execution. You configure them as below:

    IAzureStorageCommandQueueProcessorFactory listenerFactory = resolver.Resolve<IAzureStorageCommandQueueProcessorFactory>();
    Task longRunningTask = listenerFactory.Start<AddCommand, AddCommand>(queue, cancellationTokenSource.Token);

You need to manage the task execution and make sure your Web Job doesn't quit but that's a pretty standard pattern for a Web Job that I won't cover here.

## Support

If you get stuck log a GitHub issue or catch me over on Twitter at [@azuretrenches](https://twitter.com/azuretrenches) and I'll help if I can.