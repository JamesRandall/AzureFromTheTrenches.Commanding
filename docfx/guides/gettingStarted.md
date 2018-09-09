# Getting Started

Firstly install the nuget package for commanding:

```
Install-Package AzureFromTheTrenches.Commanding
```

As an example let's create a command that adds two numbers together and returns a result:

```c#
public class MathResult
{
    public int Value { get; set; }
}

public class AddCommand : ICommand<MathResult>
{
    public int FirstNumber { get; set; }

    public int SecondNumber { get; set; }
}
```

Commands are acted on by handlers and our add handler looks like this:

```c#
public AddCommandHandler : ICommandHandler<AddCommand, MathResult>
{
    public Task<MathResult> ExecuteAsync(AddCommand command, MathResult previousResult)
    {
        return new MathResult {
            Value = command.FirstNumber + command.SecondNumber
        };
    }
}
```

Having defined our command, result and handler, we need to register these with the commanding system. If you're just writing a console app you can do this in Program.cs but for more realistic usage you'd do this where you configure your IoC container - it's handy to think of command registrations as just another part of your applications configuration, besides which you'll need access to the container. The example below demonstrates registration with the Microsoft ASP.Net Core Service Provider:

In your Startup.cs add a private variable to hold the IServiceProvider which we'll need to use when registering the commanding system.

`private IServiceProvider _serviceProvider;`

Now in your `Configure(IApplicationBuilder app, IHostingEnvironment env)` method assign this variable with the app.ApplicationServices.

`_serviceProvider = app.ApplicationServices;`

Once this is done, we can get the commanding framework registered within the `ConfigureServices(IServiceCollection services)` method. Add the following lines of code.

```c#
var adapter = new CommandingDependencyResolverAdapter(
	(fromType, toInstance) => services.AddSingleton(fromType, toInstance),
	(fromType, toType) => services.AddTransient(fromType, toType),
	(resolveTo) => _serviceProvider.GetService(resolveTo));

var registry = adapter.AddCommanding();
```

The commanding framework is now registered, and we can register our handler(s) using the registry variable

`registry.Register<AddCommandHandler>();`

The _CommandingDependencyResolverAdapter_ class is an adapter that allows the framework to be registed with any IoC container and the _AddCommanding_ method registers the injectable commanding interfaces and returns an _ICommandRegistry_ interface that allows you to register handlers - you only need to register a handler, the framework will figure out the rest and registration uses a fluent API style for concise readable code.

To dispatch our command we need to get hold of the ICommandDispatcher interface and send the command. We'll do that and output the result to the console:

```c#
ICommandDispatcher commandDispatcher = dependencyResolver.ServiceProvider.GetService<ICommandDispatcher>();
MathResult mathResult = await commandDispatcher.DispatchAsync(new AddCommand { FirstNumber = 5, SecondNumber = 6});
Console.WriteLine(mathResult.Value); // hopefully says 11
```

And for simple usage that's it. This example is a bit contrived as we're resolving dependencies by hand and there's a lot of boilerplate to add two numbers together but in real world scenarios all you really need to do is register your command handlers in the appropriate place, for example if you're using ASP.Net Core then all the dependency injection boilerplate is in place.
