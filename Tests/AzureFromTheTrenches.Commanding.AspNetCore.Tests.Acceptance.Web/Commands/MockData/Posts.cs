using System;
using System.Collections.Concurrent;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.MockData
{
    public static class Posts
    {
        public static readonly ConcurrentBag<Post> Items = new ConcurrentBag<Post>
        {
            new Post()
            {
                Id = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Title = "A random post",
                Body = "Some text for the post"
            },
            new Post()
            {
                Id = Constants.PresetUserAuthoredPostId,
                AuthorId = Constants.UserId,
                Title = "Authored user post",
                Body = "Authored by logged in user"
            }
        };
    }
}
