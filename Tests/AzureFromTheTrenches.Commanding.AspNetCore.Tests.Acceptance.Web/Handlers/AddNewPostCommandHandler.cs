using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.MockData;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Handlers
{
    public class AddNewPostCommandHandler : ICommandHandler<AddNewPostCommand, Guid>
    {
        public Task<Guid> ExecuteAsync(AddNewPostCommand command, Guid previousResult)
        {
            Post newPost = new Post
            {
                AuthorId = command.AuthorId,
                Body = command.Body,
                Id = Guid.NewGuid(),
                Title = command.Title
            };
            Posts.Items.Add(newPost);
            return Task.FromResult(newPost.Id);
        }
    }
}
