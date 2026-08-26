using System.Net;
using NSubstitute;
using ScriptBee.MCP.Auth;

namespace ScriptBee.MCP.Tests.Auth;

public class GatewayAuthHandlerTests
{
    [Fact]
    public async Task SendAsync_AttachesBearerToken_WhenTokenIsAvailable()
    {
        // Arrange
        var tokenService = Substitute.For<IOidcTokenService>();
        tokenService
            .GetAccessTokenAsync(Arg.Any<CancellationToken>())
            .Returns("sample-valid-token");

        var innerHandler = new EchoHandler();
        var handler = new GatewayAuthHandler(tokenService) { InnerHandler = innerHandler };

        using var client = new HttpClient(handler);
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "http://localhost:5117/api/projects"
        );

        // Act
        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        innerHandler.CapturedRequest.ShouldNotBeNull();
        innerHandler.CapturedRequest.Headers.Authorization.ShouldNotBeNull();
        innerHandler.CapturedRequest.Headers.Authorization.Scheme.ShouldBe("Bearer");
        innerHandler.CapturedRequest.Headers.Authorization.Parameter.ShouldBe("sample-valid-token");
    }

    [Fact]
    public async Task SendAsync_DoesNotAttachAuthorizationHeader_WhenNoTokenIsAvailable()
    {
        // Arrange
        var tokenService = Substitute.For<IOidcTokenService>();
        tokenService.GetAccessTokenAsync(Arg.Any<CancellationToken>()).Returns((string?)null);

        var innerHandler = new EchoHandler();
        var handler = new GatewayAuthHandler(tokenService) { InnerHandler = innerHandler };

        using var client = new HttpClient(handler);
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "http://localhost:5117/api/projects"
        );

        // Act
        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        innerHandler.CapturedRequest.ShouldNotBeNull();
        innerHandler.CapturedRequest.Headers.Authorization.ShouldBeNull();
    }

    private sealed class EchoHandler : HttpMessageHandler
    {
        public HttpRequestMessage? CapturedRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            CapturedRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
