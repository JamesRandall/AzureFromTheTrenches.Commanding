using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Infrastructure;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses;
using Newtonsoft.Json;
using Xbehave;
using Xunit;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance
{
    public class ProfileControllerTests : AbstractControllerTestBase
    {
        [Scenario]
        public void ShouldRetrieveLoggedOnUsersPosts(string requestUrl, HttpResponseMessage response, Post post)
        {
            "Given a request for the current users profiles"
                .x(() => requestUrl = "/api/profile/posts");
            "When the API call is made"
                .x(async () => response = await HttpClient.GetAsync(requestUrl));
            "Then the response is 200"
                .x(() => Assert.Equal(HttpStatusCode.OK, response.StatusCode));
            "And only one post is returned"
                .x(async () =>
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Post[] posts = JsonConvert.DeserializeObject<Post[]>(json);
                    Assert.NotNull(posts);
                    post = posts.Single(x => x.Id == Constants.PresetUserAuthoredPostId); // tests run in parallel and others might add to the bag
                });
            "And the post is the expected post"
                .x(() =>
                {
                    Assert.Equal(Constants.UserId, post.AuthorId);
                    Assert.Equal("Authored user post", post.Title);
                    Assert.Equal("Authored by logged in user", post.Body);
                });
        }
    }
}
