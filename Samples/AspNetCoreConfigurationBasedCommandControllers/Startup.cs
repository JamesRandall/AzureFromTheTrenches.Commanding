using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Loader;
using AspNetCoreConfigurationBasedCommandControllers.Commands;
using AspNetCoreConfigurationBasedCommandControllers.Commands.Responses;
using AspNetCoreConfigurationBasedCommandControllers.Handlers;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore;
using AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
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
            CommandingDependencyResolver resolver = new CommandingDependencyResolver(
                (fromType, toInstance) => services.AddSingleton(fromType, toInstance),
                (fromType, toType) => services.AddTransient(fromType, toType),
                (resolveTo) => _serviceProvider.GetService(resolveTo));
            ICommandRegistry registry = resolver.UseCommanding();
            registry.Register<UpdatePropertyValueCommandHandler>();
            registry.Register<GetPropertyValueQueryHandler>();

            services
                .AddMvc()
                .AddAspNetCoreCommanding(cfg =>
                {
                    cfg
                        .Controller("PropertyValue", actions =>
                        {
                            actions
                                .Action<GetPropertyValueQuery, PropertyValue>(HttpMethod.Get)
                                .Action<UpdatePropertyValueCommand>(HttpMethod.Put);
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

            //app.ConfigureCommandRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();
        }
    }
}
