using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.MockData
{
    public static class Posts
    {
        public static readonly List<Post> Items = new List<Post>
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
                Id = Guid.NewGuid(),
                AuthorId = Constants.UserId,
                Title = "Another post",
                Body = "Authored by logged in user"
            }
        };
    }
}
