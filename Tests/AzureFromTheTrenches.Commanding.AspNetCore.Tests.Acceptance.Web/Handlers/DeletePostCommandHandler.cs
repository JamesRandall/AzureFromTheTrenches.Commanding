using System.Net;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.MockData;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Handlers
{
    public class DeletePostCommandHandler : ICommandHandler<DeletePostCommand>
    {
        public Task ExecuteAsync(DeletePostCommand command)
        {
            if (!Posts.Items.TryRemove(command.PostId, out Post _))
            {
                // Normally you would implement a validation pattern for command handlers
                // and have the mediator interpret that and throw these exceptions.
                // Doing this here is bad practice as it forms a link between a handler and
                // it's host however in this case it servers to make the acceptance tests
                // simpler.
                throw new RestApiException(HttpStatusCode.NotFound);
            }

            return Task.CompletedTask;
        }
    }
}
