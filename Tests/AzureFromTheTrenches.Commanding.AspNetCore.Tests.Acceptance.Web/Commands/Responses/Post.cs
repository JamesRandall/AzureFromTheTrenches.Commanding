using System;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses
{
    public class Post
    {
        public Guid Id { get; set; }

        public Guid AuthorId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }
    }
}
