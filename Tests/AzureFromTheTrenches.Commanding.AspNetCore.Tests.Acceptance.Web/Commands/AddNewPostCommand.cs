using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands
{
    /// <summary>
    /// The result is the ID of the new post
    /// </summary>
    public class AddNewPostCommand : ICommand<Guid>
    {
        [SecurityProperty]
        public Guid AuthorId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }
    }
}
