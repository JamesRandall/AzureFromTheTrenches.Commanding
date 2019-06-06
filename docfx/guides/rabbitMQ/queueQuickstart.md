# RabbitMQ Quickstart

This quickstart walks through how to configure the commanding system to dispatch commands over RabbitMQ either from a docker container or local installation and then execute them from Queues and Subscriptions.

A simple sample is available here:

[https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/RabbitMQDispatchAndDequeue](https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/RabbitMQDispatchAndDequeue)

## Dispatching to a RabbitMQ Queue

To get started you'll need to install a NuGet package:

    Install-Package AzureFromTheTrenches.Commanding.RabbitMQ

And then register the RabbitMq commanding system with your IoC container:

    ICommandRegistry registry = resolver.AddCommanding();
    resolver.AddRabbitMq();

Given a QueueClient we can configure a command to be dispatched to it as shown below:

    QueueClient client = new QueueClient(new RabbitMQClientOptions() { Queue = "myqueue" });
    commandRegistry.Register<AddCommand>(client.CreateCommandDispatcherFactory());

    RabbitMQClientOptions by default will connect to a RabbitMQ instance on localhost on standard port.

Dispatching a command is the same as ever - where and how the command is executed is completely transparent to the dispatcher:

    await commandDispatcher.DispatchAsync(new AddCommand { FirstNumber = 5, SecondNumber = 6});

## Executing Queued Commands from Queues and Subscriptions

Like for dispatch you'll first need to install a NuGet package:

    Install-Package AzureFromTheTrenches.Commanding.RabbitMQ

And then we need to register the commanding system with our IoC container along with the command and its handler:

    ICommandRegistry registry = resolver.AddCommanding();
    resolver.AddQueues().AddRabbitMq();
    registry.Register<AddCommand, AddCommandHandler>();

And finally we create a RabbitMq command queue processor:

    QueueClient client = new QueueClient(new RabbitMQClientOptions() { Queue = "myqueue" });
    IServiceBusCommandQueueProcessor commandQueueProcessor = factory.Create<AddCommand>(client);

Note that you need to keep a reference to the commandQueueProcessor if you don't want it to shut down.
