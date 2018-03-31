using System;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Loader;
using AspNetCoreConfigurationBasedCommandControllers.Commands;
using AspNetCoreConfigurationBasedCommandControllers.Commands.Responses;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            //registry.Register<>()
            resolver.UseAspNetCoreCommanding();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureCommandRouting(cfg =>
            {
                cfg
                    .Controller("PropertyValue", actions =>
                        {
                            actions.Action<GetPropertyValueQuery, PropertyValue>(HttpMethod.Get);
                        });
            });

            app.UseMvc();
        }
    }
}
