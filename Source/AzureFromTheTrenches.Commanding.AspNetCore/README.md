# AzureFromTheTrenches.Commanding ASP .NET Core Extensions

This extension package for allows commands to be exposed as REST endpoints using a configuration based approach removing all the boilerplate associated with implementing controllers and helping to enforce a consistent implementation and API expression.

Controllers managed by this package can be mixed with hand written controllers in a project - you don't need to exclusively use this approach within a project.

## Installation

Add the NuGet package to your ASP.Net Core project:

    Install-Package AzureFromTheTrenches.Commanding.AspNetCore

If you're using Swashbuckle and your commands contain sensitive properties (see Protecting Sensitive Properties below) it is recommended that you also add the Swashbuckle package - your properties will be secure and tamper proof without this package but adding this package and configuring it will stop those properties from being exposed in the Swagger document:

    Install-Package AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle

## Configuration

Controllers and actions are mapped to commands by an extension method on the IMvcBuilder returned from the .AddMvc() method that in the typical ASP.Net Core startup project can be found in ConfigureServices.

Let's assume we have a command like the below:

    public class AddToBasketCommand : ICommand
    {
        public Guid ProductId { get; set; }
    }

And we want to expose this command as a POST request on the route /api/Basket then a simple configuration block for this might look like:

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket", controller => controller
                    .Action<AddToBasketCommand>(HttpMethod.Post)
                )
            );
    }

Although other configuration options exist that's all that is needed to map a command to an endpoint using the default conventions. The configuration API is fluent so we can add a GetBaskketQuery command as follows:

    public class GetBasketQuery : ICommand<Basket>
    {

    }

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket", controller => controller
                    .Action<GetBasketQuery>(HttpMethod.Get)
                    .Action<AddToBasketCommand>(HttpMethod.Post)
                )
            );
    }

And we can add another controller to get the users profile in a similar way:

    public class GetUserProfileQuery : ICommand<UserProfile>
    {

    }

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket", controller => controller
                    .Action<GetBasketQuery>(HttpMethod.Get)
                    .Action<AddToBasketCommand>(HttpMethod.Post)
                )
                .Controller("UserProfile", controller => controller
                    .Action<GetUserProfileQuery>(HttpMethod.Get)
                )
            );
    }

In the following sections we'll explore some of the additional options available when configuring the framework.

### Controllers



### Actions

Get and delete default to FromRoute while put and post defaule to FromBody


### Adding Attributes / Filters


### Claims Mapping


### Protecting Sensitive Properties


## How It Works

### Introducing a Custom Command Dispatcher / Mediator

### Customizing the Compiled Controllers


