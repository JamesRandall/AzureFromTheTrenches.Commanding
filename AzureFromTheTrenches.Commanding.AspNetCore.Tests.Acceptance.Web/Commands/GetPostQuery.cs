using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands
{
    public class GetPostQuery : ICommand<Post>
    {
        [SecurityProperty]
        public Guid UserId { get; set; }

        public Guid PostId { get; set; }
    }
}
