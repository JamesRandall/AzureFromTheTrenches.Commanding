using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Handlers;
using AzureFromTheTrenches.Commanding.AzureStorage;
using AzureFromTheTrenches.Commanding.Queue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web
{
    public class Startup
    {
        private IServiceProvider _serviceProvider;

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

            CommandingDependencyResolver resolver = new CommandingDependencyResolver(
                (fromType, toInstance) => services.AddSingleton(fromType, toInstance),
                (fromType, toType) => services.AddTransient(fromType, toType),
                (resolveTo) => _serviceProvider.GetService(resolveTo));            
            ICommandRegistry registry = resolver.UseCommanding();
            resolver.UseQueues().UseAzureStorageCommanding();

            // Register our command handlers as per usual
            registry.Register<AddCommandHandler>();
            registry.Register<GetPostsQueryHandler>();
            registry.Register<GetPostQueryHandler>();

            // Register our expensive operation command to be sent to a queue
            registry.Register<ExpensiveOperationCommand>(
                CloudQueueDispatcherFactory.Create(storageAccountConnectionString, expensiveOperationQueueName));

            // Register our commands as services. This results in an API where the AddCommand, GetPostsQuery and
            // GetPostQuery are handled as GET requests and executed immediately in process by the registered
            // handlers while the ExpensiveOperationCommand is exposed as a POST operation and results in the
            // command being placed on a queue
            services
                .AddMvc()
                .AddAspNetCoreCommanding(cfg => cfg
                    .Controller("Add", controller =>
                        controller.Action<AddCommand>(HttpMethod.Get))
                    .Controller("Posts", controller => controller
                        .Action<GetPostsQuery>(HttpMethod.Get)
                        .Action<GetPostQuery, FromRouteAttribute>(HttpMethod.Get, "{postId}")
                        )
                    .Controller("ExpensiveOperation", controller => controller
                        .Action<ExpensiveOperationCommand>(HttpMethod.Post))
                    .Claims(mapper => mapper
                        .MapClaimToPropertyName("UserId", "UserId"))
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
