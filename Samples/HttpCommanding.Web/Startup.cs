using System;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection;
using HttpCommanding.Model.Commands;
using HttpCommanding.Model.Results;
using HttpCommanding.Web.Auditors;
using HttpCommanding.Web.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HttpCommanding.Web
{
    public class Startup
    {
        private IMicrosoftDependencyInjectionCommandingResolver _commandingDependencyResolver;

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
            services.AddMvc();
            _commandingDependencyResolver = new MicrosoftDependencyInjectionCommandingResolver(services);
            _commandingDependencyResolver.UseCommanding()
                .Register<UpdatePersonalDetailsCommandHandler>();
            _commandingDependencyResolver.UseExecutionCommandingAuditor<ExecutionAuditor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _commandingDependencyResolver.ServiceProvider = app.ApplicationServices;
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
