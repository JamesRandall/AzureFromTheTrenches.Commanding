using System;
using System.Net.Http;
using AspNetCoreConfigurationBasedCommandControllers.Commands;
using AspNetCoreConfigurationBasedCommandControllers.Filters;
using AspNetCoreConfigurationBasedCommandControllers.Handlers;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore;
using AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace AspNetCoreConfigurationBasedCommandControllers
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
            CommandingDependencyResolverAdapter resolver = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => services.AddSingleton(fromType, toInstance),
                (fromType, toType) => services.AddTransient(fromType, toType),
                (resolveTo) => _serviceProvider.GetService(resolveTo));

            ICommandRegistry registry = resolver.AddCommanding();
            registry.Register<UpdatePropertyValueCommandHandler>();
            registry.Register<GetPropertyValueQueryHandler>();
            registry.Register<GetMessageQueryHandler>();

            services
                .AddMvc(cfg => cfg.Filters.Add<ClaimsInjectionFilter>())
                .AddAspNetCoreCommanding(cfg =>
                {
                    cfg
                        // Define RESTful controllers and actions based on commands
                        .Controller("PropertyValue",
                            attributes => attributes.Attribute<AuthorizeAttribute>(),
                            actions =>  actions
                                .Action<GetPropertyValueQuery>(HttpMethod.Get)
                                .Action<UpdatePropertyValueCommand>(HttpMethod.Put))
                        .Controller("Message", actions => { actions.Action<GetMessageQuery>(HttpMethod.Get); })
                        // Configure claims to automatically populate properties on commands
                        .Claims(mapping =>
                        {
                            mapping
                                .MapClaimToCommandProperty<GetPropertyValueQuery>("UserId", cmd => cmd.MisspelledUserId)
                                .MapClaimToPropertyName("UserId", "UserId");
                        });
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
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
