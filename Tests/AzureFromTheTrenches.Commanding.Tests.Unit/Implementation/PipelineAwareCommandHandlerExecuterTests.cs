using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Implementation;
using AzureFromTheTrenches.Commanding.Tests.Unit.TestModel;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.Implementation
{
    public class PipelineAwareCommandHandlerExecuterTests
    {
        [Fact]
        public async Task ExecutesHandlerWithResult()
        {
            // Arrange
            PipelineAwareCommandHandlerExecuter testSubject = new PipelineAwareCommandHandlerExecuter();
            SimpleCommandPipelineAwareHandler handler = new SimpleCommandPipelineAwareHandler();
            SimpleCommand command = new SimpleCommand();

            // Act
            PipelineAwareCommandHandlerResult<SimpleResult> result = await testSubject.ExecuteAsync(handler, command, null, CancellationToken.None);

            // Assert
            Assert.Single(result.Result.Handlers);
            Assert.Equal(typeof(SimpleCommandPipelineAwareHandler), result.Result.Handlers.Single());
        }

        [Fact]
        public async Task ExecutesCancellableHandlerWithResult()
        {
            // Arrange
            PipelineAwareCommandHandlerExecuter testSubject = new PipelineAwareCommandHandlerExecuter();
            CancellableSimpleCommandPipelineAwareHandler handler = new CancellableSimpleCommandPipelineAwareHandler();
            SimpleCommand command = new SimpleCommand();

            // Act
            PipelineAwareCommandHandlerResult<SimpleResult> result = await testSubject.ExecuteAsync(handler, command, null, CancellationToken.None);

            // Assert
            Assert.Single(result.Result.Handlers);
            Assert.Equal(typeof(CancellableSimpleCommandPipelineAwareHandler), result.Result.Handlers.Single());
        }
    }
}
