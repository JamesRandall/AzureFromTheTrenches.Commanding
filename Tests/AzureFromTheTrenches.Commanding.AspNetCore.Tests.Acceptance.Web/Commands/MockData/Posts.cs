using System;
using System.Collections.Concurrent;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.MockData
{
    public static class Posts
    {
        public static readonly ConcurrentDictionary<Guid, Post> Items;

        static Posts()
        {
            Items = new ConcurrentDictionary<Guid, Post>();
            Items[Constants.PresetPostId] = new Post()
            {
                Id = Constants.PresetPostId,
                AuthorId = Guid.NewGuid(),
                Title = "A preset post with a random author",
                Body = "Some text for the post"
            };
            Items[Constants.PresetUserAuthoredPostId] = new Post()
            {
                Id = Constants.PresetUserAuthoredPostId,
                AuthorId = Constants.UserId,
                Title = "Authored user post",
                Body = "Authored by logged in user"
            };
        }
    }
}
