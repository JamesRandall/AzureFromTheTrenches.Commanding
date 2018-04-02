using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Handlers
{
    public class GetPostsQueryHandler : ICommandHandler<GetPostsQuery, IReadOnlyCollection<Post>>
    {
        public Task<IReadOnlyCollection<Post>> ExecuteAsync(GetPostsQuery command, IReadOnlyCollection<Post> previousResult)
        {
            throw new NotImplementedException();
        }
    }
}
