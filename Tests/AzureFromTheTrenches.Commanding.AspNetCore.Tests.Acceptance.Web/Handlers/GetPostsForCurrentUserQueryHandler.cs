using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.MockData;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Handlers
{
    public class GetPostsForCurrentUserQueryHandler : ICommandHandler<GetPostsForCurrentUserQuery, IReadOnlyCollection<Post>>
    {
        public Task<IReadOnlyCollection<Post>> ExecuteAsync(GetPostsForCurrentUserQuery command, IReadOnlyCollection<Post> previousResult)
        {
            return Task.FromResult(
                (IReadOnlyCollection<Post>) Posts.Items.Values.Where(x => x.AuthorId == command.UserId).ToArray());
        }
    }
}
