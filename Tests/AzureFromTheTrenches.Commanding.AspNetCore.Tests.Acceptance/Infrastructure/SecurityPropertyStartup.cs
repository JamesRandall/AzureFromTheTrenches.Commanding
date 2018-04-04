using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Infrastructure
{
    public class SecurityPropertyStartup : Startup
    {
        public SecurityPropertyStartup(IConfiguration configuration) : base(configuration)
        {
            CommandLog = new List<ICommand>();
            CommandDispatcher = new CaptureCommandDispatcher();
        }

        public override void ConfigureClaimsMapping(IClaimsMappingBuilder mapper)
        {
            // don't set up claims mapping
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.Replace(new ServiceDescriptor(typeof(ICommandDispatcher),
                CommandDispatcher));
        }

        protected ICommandDispatcher CommandDispatcher { get; }

        protected List<ICommand> CommandLog { get; }
    }
}
