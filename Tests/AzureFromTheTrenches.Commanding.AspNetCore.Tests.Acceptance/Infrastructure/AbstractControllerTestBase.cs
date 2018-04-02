using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Infrastructure
{
    public abstract class AbstractControllerTestBase : IDisposable
    {
        private readonly TestServer _testServer;

        protected AbstractControllerTestBase()
        {
            //Assembly startupAssembly = typeof(Startup).Assembly;
            IWebHostBuilder webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>();

            _testServer = new TestServer(webHostBuilder);

            HttpClient = _testServer.CreateClient();
            HttpClient.BaseAddress = new Uri("http://localhost:58690");
        }

        protected HttpClient HttpClient { get; }

        public void Dispose()
        {
            HttpClient?.Dispose();
            _testServer?.Dispose();            
        }
    }
}
