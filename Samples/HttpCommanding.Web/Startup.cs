using System;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.DependencyResolver.MicrosoftNetStandard;
using HttpCommanding.Model.Commands;
using HttpCommanding.Model.Results;
using HttpCommanding.Web.Actors;
using InMemoryCommanding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HttpCommanding.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            IServiceProvider serviceProvider = null;
            services.AddMvc();
            CommandingDependencyResolver dependencyResolver = services.GetCommandingDependencyResolver(() => serviceProvider);

            Options options = new Options
            {
                CommandActorContainerRegistration = type => services.AddTransient(type, type)
            };
            CommandingDependencies.UseCommanding(dependencyResolver, options)
                .Register<UpdatePersonalDetailsCommand, UpdateResult, UpdatePersonalDetailsCommandActor>();
            serviceProvider = services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
