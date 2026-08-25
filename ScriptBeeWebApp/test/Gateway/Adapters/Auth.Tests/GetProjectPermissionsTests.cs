using System.Net;
using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using VeriJson;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ScriptBee.Adapters.Auth.Tests;

public class GetProjectPermissionsTests : IDisposable
{
    private readonly WireMockServer _server = WireMockServer.Start();

    public void Dispose()
    {
        _server.Stop();
    }

    [Fact]
    public async Task GivenNonSuccessResponse_ShouldReturnEmptyPermissions()
    {
        // Arrange
        _server
            .Given(Request.Create().WithPath("/api/project-permissions").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.InternalServerError));

        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(GetProjectPermissions.ClientName)
            .Returns(
                new HttpClient
                {
                    BaseAddress = new Uri($"{_server.Urls[0]}/api/project-permissions"),
                }
            );

        var sut = new GetProjectPermissions(httpClientFactory);

        // Act
        var permissions = await sut.GetPermissions(
            ProjectId.FromValue("project-123"),
            new UserId("user-123"),
            [new UserGroup("admins"), new UserGroup("operators")],
            new UserRole("owner"),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Empty(permissions);

        var log = Assert.Single(_server.LogEntries);
        Assert.Equal("/api/project-permissions", log.RequestMessage?.Path);
    }

    [Fact]
    public async Task GivenPermissionsAreReturned_ShouldReturnThem()
    {
        // Arrange
        var expectedPermissions = new[] { "permission-1", "permission-2" };

        _server
            .Given(Request.Create().WithPath("/api/project-permissions").UsingPost())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBodyAsJson(new { result = expectedPermissions })
            );

        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(GetProjectPermissions.ClientName)
            .Returns(
                new HttpClient
                {
                    BaseAddress = new Uri($"{_server.Urls[0]}/api/project-permissions"),
                }
            );

        var sut = new GetProjectPermissions(httpClientFactory);

        // Act
        var permissions = await sut.GetPermissions(
            ProjectId.FromValue("project-123"),
            new UserId("user-123"),
            [new UserGroup("admins"), new UserGroup("operators")],
            new UserRole("owner"),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(expectedPermissions, permissions);

        var log = Assert.Single(_server.LogEntries);
        Assert.Equal("/api/project-permissions", log.RequestMessage?.Path);

        var body = log.RequestMessage?.Body!;
        body.Should()
            .BeEquivalentTo(
                """
                {
                    "input": {
                        "subject": {
                            "user_id": "user-123",
                            "groups": ["admins", "operators"]
                        },
                        "resource": {
                            "type": "project",
                            "id": "project-123",
                            "role": "owner"
                        }
                    }
                }
                """
            );
    }
}
