using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Infrastructure
{
    public abstract class AbstractControllerTestBase<TStartup> : IDisposable where TStartup : class
    {
        private readonly TestServer _testServer;

        protected AbstractControllerTestBase()
        {
            //Assembly startupAssembly = typeof(Startup).Assembly;
            IWebHostBuilder webHostBuilder = new WebHostBuilder()
                .UseStartup<TStartup>();
            _testServer = new TestServer(webHostBuilder);
            
            HttpClient = _testServer.CreateClient();
            HttpClient.BaseAddress = new Uri("http://localhost:58690");
        }

        protected HttpClient HttpClient { get; }

        protected ICommandDispatcher CommandDispatcher => (ICommandDispatcher)_testServer.Host.Services.GetService(typeof(ICommandDispatcher));

        public void Dispose()
        {
            HttpClient?.Dispose();
            _testServer?.Dispose();            
        }
    }
}
