using System;
using System.Diagnostics;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Filters;
using AzureFromTheTrenches.Commanding.AzureStorage;
using AzureFromTheTrenches.Commanding.Queue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web
{
    public class Startup
    {
        private IServiceProvider _serviceProvider;

        private readonly CommandingRuntime _commandingRuntime = new CommandingRuntime();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string storageAccountConnectionString = Configuration["storage:connectionstring"];
            string expensiveOperationQueueName = Configuration["storage:queuename"];

            CommandingDependencyResolverAdapter resolver = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => services.AddSingleton(fromType, toInstance),
                (fromType, toType) => services.AddTransient(fromType, toType),
                (resolveTo) => _serviceProvider.GetService(resolveTo));

            // Using an instance of CommandingRuntime rather than the static helpers isolates
            // this commanding infrastructure to this startup class / instance of the web app
            // which means that acceptance tests can be run in parallel with multiple instances
            // of the ASP.Net Core test server.
            ICommandRegistry registry = _commandingRuntime.AddCommanding(resolver);

            //ICommandRegistry registry = resolver.AddCommanding();
            resolver.AddQueues().AddAzureStorageCommanding();

            // Register our command handlers using the discovery approach. Our handlers are in this assembly
            // so we just pass through our assembly
            registry.Discover(typeof(Startup).Assembly);

            // Register our expensive operation command to be sent to a queue
            registry.Register<ExpensiveOperationCommand>(
                CloudQueueDispatcherFactory.Create(storageAccountConnectionString, expensiveOperationQueueName));

            // Register our commands as REST endpoints. This results in an API where the AddCommand, GetPostsQuery,
            // GetPostsForCurrentUserQuery and GetPostQuery are handled as GET requests and executed immediately
            // in process by the registered handlers while the ExpensiveOperationCommand is exposed as a POST operation
            // and results in the command being placed on a queue
            services
                // this filter adds claims, just stops us having to wire up to a real auth provider in this sample
                .AddMvc(options => options.Filters.Add<SetClaimsFilter>())
                // this block configures our commands to be exposed on endpoints
                .AddAspNetCoreCommanding(cfg => cfg
                    .Controller("Post", controller => controller
                        .Action<GetPostsQuery>(HttpMethod.Get)
                        .Action<GetPostQuery, FromRouteAttribute>(HttpMethod.Get, "{postId}")
                        .Action<AddNewPostCommand>(HttpMethod.Post)
                        )
                    .Controller("Profile", controller => controller
                        .Action<GetPostsForCurrentUserQuery>(HttpMethod.Get, "Posts"))
                    .Controller("ExpensiveOperation", controller => controller
                        .Action<ExpensiveOperationCommand>(HttpMethod.Post))
                    .Claims(mapper => mapper
                        // this will map the claim UserId to every property on a command called UserId
                        // this can be used if a convention based approach is being used
                        .MapClaimToPropertyName("UserId", "UserId")
                        // this will map the claim UserId to the property AuthorId on the AddNewPostCommand command
                        .MapClaimToCommandProperty<AddNewPostCommand>("UserId", cmd => cmd.AuthorId))
                    .LogControllerCode(code =>
                    {
                        // this will output the code that is compiled for each controller to the debug window
                        Debug.WriteLine(code); 
                    })
                );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Acceptance Test API", Version = "v1" });
                c.AddAspNetCoreCommanding();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            _serviceProvider = app.ApplicationServices;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();
        }
    }
}
