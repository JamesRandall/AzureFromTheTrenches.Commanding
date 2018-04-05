using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Infrastructure;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses;
using Newtonsoft.Json;
using Xbehave;
using Xunit;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Security
{
    public class SecurityPropertyTests : AbstractControllerTestBase<SecurityPropertyStartup>
    {
        [Scenario]
        public void SecurePropertyShouldNotBeSetWithoutClaimsMapping(string requestUrl, Post post)
        {
            "Given a request for the current users profiles"
                .x(() => requestUrl = "/api/profile/posts");
            "When the API call is made"
                .x(async () => await HttpClient.GetAsync(requestUrl));
            "Then the captured command contains an empty user ID"
                .x(() =>
                {
                    CaptureCommandDispatcher commandDispatcher = (CaptureCommandDispatcher) CommandDispatcher;
                    Assert.Equal(1, commandDispatcher.CommandLog.Count);
                    Assert.IsType<GetPostsForCurrentUserQuery>(commandDispatcher.CommandLog.Single());
                    Assert.Equal(Guid.Empty, ((GetPostsForCurrentUserQuery)commandDispatcher.CommandLog.Single()).UserId);
                });
        }

        [Scenario]
        public void AttemptToPokeInValueViaPostRequestFails(string requestUrl)
        {
            "Given a URL with a post body with a secure property"
                .x(() => requestUrl = "/api/securityTest");
            "When the request attempts to set the property via a POST operation"
                .x(async () => await HttpClient.PostAsync(requestUrl,
                    new StringContent(JsonConvert.SerializeObject(
                        new SecurityTestCommand
                        {
                            AnotherPieceOfData = "something",
                            SensitiveData = "shouldbeblocked"
                        }), Encoding.UTF8, "application/json")));
            "Then the captured command ignores the data in the payload and the command contains a null SensitiveData property"
                .x(() =>
                {
                    CaptureCommandDispatcher commandDispatcher = (CaptureCommandDispatcher)CommandDispatcher;
                    Assert.Equal(1, commandDispatcher.CommandLog.Count);
                    Assert.IsType<SecurityTestCommand>(commandDispatcher.CommandLog.Single());
                    Assert.Null(((SecurityTestCommand)commandDispatcher.CommandLog.Single()).SensitiveData);
                });
        }

        [Scenario]
        public void AttemptToPokeInValueViaGetQueryParamaterRequestFails(string requestUrl)
        {
            "Given a URL with a secure property"
                .x(() => requestUrl = "/api/securityTest/asQueryParam?SensitiveData=shouldbeblocked&AnotherPieceOfData=something");
            "When the request attempts to set the property via a GET operation"
                .x(async () => await HttpClient.GetAsync(requestUrl));
            "Then the captured command ignores the data in the payload and the command contains a null SensitiveData property"
                .x(() =>
                {
                    CaptureCommandDispatcher commandDispatcher = (CaptureCommandDispatcher)CommandDispatcher;
                    Assert.Equal(1, commandDispatcher.CommandLog.Count);
                    Assert.IsType<SecurityTestCommand>(commandDispatcher.CommandLog.Single());
                    Assert.Null(((SecurityTestCommand)commandDispatcher.CommandLog.Single()).SensitiveData);
                });
        }


    }
}
