using System.Net;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ScriptBee.Web.Auth;
using ScriptBee.Web.Auth.Contracts;
using VeriJson;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ScriptBee.Web.Tests.Auth;

public class AuthorizeExternallyTests : IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start();

    public void Dispose()
    {
        _server.Stop();
    }

    [Fact]
    public async Task GivenValidExternalAuthorizationRequest_ShouldPostExpectedPayloadToAuthorizationService()
    {
        // Arrange
        _server
            .Given(Request.Create().WithPath("/api/authorize").UsingPost())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBodyAsJson(new { result = true })
            );

        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient("ExternalAuthorizationClient")
            .Returns(new HttpClient { BaseAddress = new Uri($"{_server.Urls[0]}/api/authorize") });

        var authorizationService = new AuthorizeExternally(
            httpClientFactory,
            Substitute.For<ILogger<ExternalAuthorizationActionAuthorizationHandler>>()
        );

        var request = new ExternalAuthorizationRequest
        {
            Input = new ExternalAuthorizationRequestInput
            {
                Subject = new ExternalAuthorizationRequestSubject
                {
                    UserId = "user-123",
                    Groups = ["admins", "operators"],
                },
                Action = "scriptbee:read",
                Resource = new ExternalAuthorizationResource
                {
                    Type = "project",
                    Id = "project-123",
                    Role = "owner",
                },
            },
        };

        // Act
        var result = await authorizationService.IsAllowed(
            request,
            TestContext.Current.CancellationToken
        );

        // Assert
        var item = Assert.Single(_server.LogEntries);
        Assert.Equal("/api/authorize", item.RequestMessage?.Path);
        var body = _server.LogEntries[0].RequestMessage?.Body!;
        body.Should()
            .BeEquivalentTo(
                """
                {
                    "input": {
                        "subject": {
                            "user_id": "user-123",
                            "groups": ["admins", "operators"]
                        },
                        "action": "scriptbee:read",
                        "resource": {
                            "type": "project",
                            "id": "project-123",
                            "role": "owner"
                        }
                    }
                }
                """
            );
        Assert.True(result);
    }
}
