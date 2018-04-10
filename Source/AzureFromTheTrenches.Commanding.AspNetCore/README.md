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

A controller can be configured with:

* A name (mandatory)
* An optional route - this is attached to the controller using the [[Route(...)]] attribute and so if you want the controller name to form part of the route it should include the {controller} component
* An optional set of attribute filters - see Attributes below
* One or more actions - see Actions below

A sample illustrating all four of the above options is shown below:

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket",
                    "api/v1/{controller}",
                    attributes => attributes.Attribute<AuthorizeAttribute>(),
                    actions => actions.Action<AddToBasketCommand>(HttpMethod.Post)
                )
            );
    }

### Actions

An action can be configured with:

* The command type expressed as a generic parameter
* A HTTP verb (mandatory)
* An optional binding attribute for the command payload e.g. [[FromBody]]. This is expressed as an optional secondary generic parameter. GET and DELETE verbs default to FromRoute while POST and PUT verbs default to FromBody
* An optional route - this is attached to the controller using the [[Route(...)]] attribute.
* An optional set of attribute filters - see Attributes below

A sample illustrating all five of the above options is shown below:

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket",
                    actions => actions
                        .Action<GetBasketQuery, FromQueryAttribute>(
                            HttpMethod.Get,
                            "MyBasket",
                            attributes => attributes.Attribute<AuthorizeAttribute>())
                )
            );
    }

### Adding Attributes / Filters


### Protecting Sensitive Properties

Commands may contain properties that you never want a caller to be able to set either via a query string, route parameter or body as doing so may cause unwanted side effects such as a data breach. Often such properties need to be populated from claims rather than by caller modifiable data.

A good example might be a user ID. If we consider our earlier AddToBasketCommand command in order for the product to be added to the correct basket it is likely that the command handler and downstream systems will need to be aware of which users basket we are dealing with and so a more realistic representation of this class might be:

    public class AddToBasketCommand : ICommand
    {
        [SecurityProperty]
        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }
    }

By marking the UserId property with the attribute [SecurityProperty] that property cannot be set by sending data to the API endpoint.

Additionally by adding and configuring the AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle package the properties will never be shown in the Swagger definition. The package and Swashbuckle are configured together in ConfigureServices by using the AddSwaggerGen options:

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        // ... configure REST API commanding

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            c.AddAspNetCoreCommanding();
        });
    }


### Claims Mapping

Properties on commands can be set by mapping the values from claims. If we consider our previous example:

    public class AddToBasketCommand : ICommand
    {
        [SecurityProperty]
        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }
    }

It is quite likely that the value for the UserId property will actually be found in a claim. If you stick to a convention based approach (e.g. all user IDs on all commands are called UserId) then a single claim mapping can deal with all commands that contain a UserID property. Assuming our claim containing the user ID is called UniqueUserId (I've just differentiated the name to make the example clear) then this can be setup as shown in the example below:

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket", controller => controller
                    .Action<AddToBasketCommand>(HttpMethod.Post))
                .Claims(mapping => mapping.MapClaimToPropertyName("UniqueUserId", "UserId))
            );
    }

If you have some commands that don't follow the pattern then the generic claim mapping can be overridden by setting up a mapping for a specific command type. For example lets assume our GetBasketQuery has a property called BasketUserId:

    public class GetBasketQuery : ICommand<Basket>
    {
        [SecurityProperty]
        public Guid BasketUserId { get; set; }
    }

Then we can set up a specific mapping for the GetBasketQuery command as follows:

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket", controller => controller
                    .Action<AddToBasketCommand>(HttpMethod.Post))
                .Claims(mapping => mapping
                    .MapClaimToPropertyName("UniqueUserId", "UserId)
                    .MapClaimToCommandProperty<GetBasketQuery>("UserId", cmd => cmd.BasketUserId))
            );
    }

Claims are mapped during model binding and so are present when validation takes place.

## How It Works



### Introducing a Custom Command Dispatcher / Mediator

### Customizing the Compiled Controllers


