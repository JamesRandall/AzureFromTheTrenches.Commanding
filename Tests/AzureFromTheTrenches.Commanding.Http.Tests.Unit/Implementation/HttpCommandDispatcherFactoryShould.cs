using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Http.Implementation;
using Moq;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Http.Tests.Unit.Implementation
{
    public class HttpCommandDispatcherFactoryShould
    {
        [Fact]
        public void CreateADispatcher()
        {
            Mock<IHttpCommandSerializer> serializer = new Mock<IHttpCommandSerializer>();
            Mock<IUriCommandQueryBuilder> uriCommandQueryBuilder = new Mock<IUriCommandQueryBuilder>();
            Mock<IHttpClientProvider> httpClientProvider = new Mock<IHttpClientProvider>();

            HttpCommandDispatcherFactoryImpl testSubject = new HttpCommandDispatcherFactoryImpl(serializer.Object,
                uriCommandQueryBuilder.Object,
                httpClientProvider.Object);
            ICommandDispatcher dispatcher = testSubject.Create(new Uri("http://localhost"));

            Assert.IsType<HttpCommandDispatcher>(dispatcher);
            Assert.IsType<HttpCommandExecuter>(dispatcher.AssociatedExecuter);
        }
    }
}
