using System.Net;
using NSubstitute;
using ScriptBee.Domain.Model.User;
using ScriptBee.Web.Auth;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ScriptBee.Web.Tests.Auth;

public class GetDefaultCreatorRoleTests : IDisposable
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
            .Given(Request.Create().WithPath("/api/roles/default_creator_role").UsingGet())
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBodyAsJson(new { result = "editor" })
            );

        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient("GetDefaultCreatorRole")
            .Returns(
                new HttpClient
                {
                    BaseAddress = new Uri($"{_server.Urls[0]}/api/roles/default_creator_role"),
                }
            );

        var getDefaultCreatorRole = new GetDefaultCreatorRole(httpClientFactory);

        // Act
        var result = await getDefaultCreatorRole.GetRole(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(new UserRole("editor"), result);
    }
}
