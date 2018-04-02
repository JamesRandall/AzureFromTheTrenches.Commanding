using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Infrastructure;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.MockData;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses;
using Newtonsoft.Json;
using Xbehave;
using Xunit;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance
{
    public class PostControllerTests : AbstractControllerTestBase
    {
        [Scenario]
        public void ShouldAddNewPost(string requestUrl, HttpResponseMessage response, Guid postId)
        {
            "Given a request for the current users profiles"
                .x(() => requestUrl = "/api/post");
            "When the API call is made"
                .x(async () =>
                {
                    const string json = "{ \"Title\": \"A new post\", \"Body\": \"With some new contenmt\" }";
                    response = await HttpClient.PostAsync(requestUrl, new StringContent(json, Encoding.UTF8, "application/json"));
                });
            "Then the response is 200"
                .x(() => Assert.Equal(HttpStatusCode.OK, response.StatusCode));
            "And a GUID for the post is returned"
                .x(async () =>
                {
                    string guidAsString = await response.Content.ReadAsStringAsync();
                    postId = JsonConvert.DeserializeObject<Guid>(guidAsString);
                    Assert.NotEqual(Guid.Empty, postId);                    
                });
            "And the author ID has been set to the current user"
                .x(() =>
                {
                    Post post = Posts.Items.Single(x => x.Id == postId);
                    Assert.Equal(post.AuthorId, Constants.UserId);
                });
        }
    }
}
