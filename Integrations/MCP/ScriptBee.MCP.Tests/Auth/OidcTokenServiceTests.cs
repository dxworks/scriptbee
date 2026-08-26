using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using ScriptBee.MCP.Auth;
using ScriptBee.MCP.Config;

namespace ScriptBee.MCP.Tests.Auth;

public class OidcTokenServiceTests
{
    [Fact]
    public async Task GetAccessTokenAsync_ReturnsStaticAccessToken_WhenConfigured()
    {
        // Arrange
        var config = new AuthConfig { AccessToken = "static-token-123" };
        var options = Options.Create(config);
        var logger = Substitute.For<ILogger<OidcTokenService>>();
        using var httpClient = new HttpClient(
            new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK))
        );
        using var service = new OidcTokenService(options, httpClient, logger);

        // Act
        var token = await service.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        // Assert
        token.ShouldBe("static-token-123");
    }

    [Fact]
    public async Task GetAccessTokenAsync_ReturnsNull_WhenAuthConfigIsMissingCredentials()
    {
        // Arrange
        var config = new AuthConfig();
        var options = Options.Create(config);
        var logger = Substitute.For<ILogger<OidcTokenService>>();
        using var httpClient = new HttpClient(
            new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK))
        );
        using var service = new OidcTokenService(options, httpClient, logger);

        // Act
        var token = await service.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        // Assert
        token.ShouldBeNull();
    }

    [Fact]
    public async Task GetAccessTokenAsync_AcquiresAndCachesToken_ViaClientCredentials()
    {
        // Arrange
        var config = new AuthConfig
        {
            Authority = "https://auth.example.com/realms/scriptbee",
            ClientId = "test-client",
            ClientSecret = "test-secret",
            Scope = "api",
        };
        var options = Options.Create(config);
        var logger = Substitute.For<ILogger<OidcTokenService>>();

        var requestCount = 0;
        var handler = new TestHttpMessageHandler(req =>
        {
            requestCount++;
            if (req.RequestUri!.PathAndQuery.Contains(".well-known/openid-configuration"))
            {
                var discoveryDoc = JsonSerializer.Serialize(
                    new
                    {
                        token_endpoint = "https://auth.example.com/realms/scriptbee/protocol/openid-connect/token",
                    }
                );
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(discoveryDoc, Encoding.UTF8, "application/json"),
                };
            }

            if (req.RequestUri!.PathAndQuery.Contains("protocol/openid-connect/token"))
            {
                var tokenDoc = JsonSerializer.Serialize(
                    new OidcTokenResponse
                    {
                        AccessToken = "acquired-access-token",
                        ExpiresIn = 3600,
                        TokenType = "Bearer",
                    }
                );
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(tokenDoc, Encoding.UTF8, "application/json"),
                };
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var httpClient = new HttpClient(handler);
        using var service = new OidcTokenService(options, httpClient, logger);

        // Act
        var firstToken = await service.GetAccessTokenAsync(TestContext.Current.CancellationToken);
        var secondToken = await service.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        // Assert
        firstToken.ShouldBe("acquired-access-token");
        secondToken.ShouldBe("acquired-access-token");
        requestCount.ShouldBe(2);
    }

    [Fact]
    public async Task GetAccessTokenAsync_UsesDirectTokenEndpoint_WhenExplicitlyConfigured()
    {
        // Arrange
        var config = new AuthConfig
        {
            Authority = "https://auth.example.com",
            ClientId = "test-client",
            ClientSecret = "test-secret",
            TokenEndpoint = "https://auth.example.com/oauth/token",
        };
        var options = Options.Create(config);
        var logger = Substitute.For<ILogger<OidcTokenService>>();

        var handler = new TestHttpMessageHandler(req =>
        {
            if (req.RequestUri!.ToString() == "https://auth.example.com/oauth/token")
            {
                var tokenDoc = JsonSerializer.Serialize(
                    new OidcTokenResponse
                    {
                        AccessToken = "token-from-direct-endpoint",
                        ExpiresIn = 3600,
                        TokenType = "Bearer",
                    }
                );
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(tokenDoc, Encoding.UTF8, "application/json"),
                };
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var httpClient = new HttpClient(handler);
        using var service = new OidcTokenService(options, httpClient, logger);

        // Act
        var token = await service.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        // Assert
        token.ShouldBe("token-from-direct-endpoint");
    }

    private sealed class TestHttpMessageHandler(
        Func<HttpRequestMessage, HttpResponseMessage> sendHandler
    ) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            return Task.FromResult(sendHandler(request));
        }
    }
}
