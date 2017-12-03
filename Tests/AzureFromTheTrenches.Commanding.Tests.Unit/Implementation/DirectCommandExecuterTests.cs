using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Implementation;
using AzureFromTheTrenches.Commanding.Tests.Unit.TestModel;
using Moq;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.Implementation
{
    public class DirectCommandExecuterTests
    {
        [Fact]
        public async Task EntersAndExitsScope()
        {
            Mock<ICommandScopeManager> commandScopeManager = new Mock<ICommandScopeManager>();
            Mock<ICommandExecuter> decoratedExecuter = new Mock<ICommandExecuter>();
            DirectCommandExecuter testSubject = new DirectCommandExecuter(commandScopeManager.Object, decoratedExecuter.Object);

            await testSubject.ExecuteAsync(new SimpleCommand());

            commandScopeManager.Verify(x => x.Enter());
            commandScopeManager.Verify(x => x.Exit());
        }

        [Fact]
        public async Task ExitsScopeAndRethrowsOnError()
        {
            Mock<ICommandScopeManager> commandScopeManager = new Mock<ICommandScopeManager>();
            Mock<ICommandExecuter> decoratedExecuter = new Mock<ICommandExecuter>();
            DirectCommandExecuter testSubject = new DirectCommandExecuter(commandScopeManager.Object, decoratedExecuter.Object);
            SimpleCommand command = new SimpleCommand();
            decoratedExecuter.Setup(x => x.ExecuteAsync(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("hello"));

            await Assert.ThrowsAsync<Exception>(async () => await testSubject.ExecuteAsync(command));

            commandScopeManager.Verify(x => x.Exit());
        }

        [Fact]
        public async Task CallsDecoratedExecuter()
        {
            Mock<ICommandScopeManager> commandScopeManager = new Mock<ICommandScopeManager>();
            Mock<ICommandExecuter> decoratedExecuter = new Mock<ICommandExecuter>();
            DirectCommandExecuter testSubject = new DirectCommandExecuter(commandScopeManager.Object, decoratedExecuter.Object);
            SimpleCommand command = new SimpleCommand();

            await testSubject.ExecuteAsync(command);

            decoratedExecuter.Verify(x => x.ExecuteAsync(command, It.IsAny<CancellationToken>()));
        }
    }
}
