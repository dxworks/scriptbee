using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Adapters.Auth.Tests;

public class ProjectTokenAuthenticationHandlerTests
{
    private readonly IOptionsMonitor<ProjectTokenAuthenticationOptions> _optionsMonitor =
        Substitute.For<IOptionsMonitor<ProjectTokenAuthenticationOptions>>();

    private readonly IValidateProjectTokenUseCase _validateProjectTokenUseCase =
        Substitute.For<IValidateProjectTokenUseCase>();

    private readonly ProjectTokenAuthenticationHandler _handler;

    public ProjectTokenAuthenticationHandlerTests()
    {
        _optionsMonitor.Get(Arg.Any<string>()).Returns(new ProjectTokenAuthenticationOptions());
        _optionsMonitor.CurrentValue.Returns(new ProjectTokenAuthenticationOptions());

        _handler = new ProjectTokenAuthenticationHandler(
            _optionsMonitor,
            NullLoggerFactory.Instance,
            UrlEncoder.Default,
            _validateProjectTokenUseCase
        );
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WhenNoTokenProvided_ReturnsNoResult()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.RequestAborted = TestContext.Current.CancellationToken;
        var scheme = new AuthenticationScheme(
            ProjectTokenAuthenticationOptions.Scheme,
            null,
            typeof(ProjectTokenAuthenticationHandler)
        );
        await _handler.InitializeAsync(scheme, httpContext);

        var result = await _handler.AuthenticateAsync();

        Assert.True(result.None);
        await _validateProjectTokenUseCase
            .DidNotReceive()
            .ValidateToken(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WhenTokenValidationFails_ReturnsFail()
    {
        const string rawToken = "sb_at_invalid123";
        var httpContext = new DefaultHttpContext();
        httpContext.RequestAborted = TestContext.Current.CancellationToken;
        httpContext.Request.Headers.Authorization = $"Bearer {rawToken}";

        var scheme = new AuthenticationScheme(
            ProjectTokenAuthenticationOptions.Scheme,
            null,
            typeof(ProjectTokenAuthenticationHandler)
        );
        await _handler.InitializeAsync(scheme, httpContext);

        _validateProjectTokenUseCase
            .ValidateToken(rawToken, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ProjectToken?>(null));

        var result = await _handler.AuthenticateAsync();

        Assert.False(result.Succeeded);
        Assert.NotNull(result.Failure);
        Assert.Equal("Invalid or expired project token.", result.Failure.Message);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WhenValidBearerToken_ReturnsSuccessWithExpectedClaims()
    {
        const string rawToken = "sb_at_valid123";
        var projectToken = new ProjectToken(
            new ProjectTokenId("tok-1"),
            ProjectId.FromValue("proj-1"),
            "hash-1",
            "CI token",
            new UserRole("editor"),
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(7)
        );

        var httpContext = new DefaultHttpContext();
        httpContext.RequestAborted = TestContext.Current.CancellationToken;
        httpContext.Request.Headers.Authorization = $"Bearer {rawToken}";

        var scheme = new AuthenticationScheme(
            ProjectTokenAuthenticationOptions.Scheme,
            null,
            typeof(ProjectTokenAuthenticationHandler)
        );
        await _handler.InitializeAsync(scheme, httpContext);

        _validateProjectTokenUseCase
            .ValidateToken(rawToken, Arg.Any<CancellationToken>())
            .Returns(projectToken);

        var result = await _handler.AuthenticateAsync();

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Principal);
        Assert.Equal("tok-1", result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal("tok-1", result.Principal.FindFirst("token_id")?.Value);
        Assert.Equal("proj-1", result.Principal.FindFirst("project_id")?.Value);
        Assert.Equal("editor", result.Principal.FindFirst("role")?.Value);
        Assert.Equal("editor", result.Principal.FindFirst(ClaimTypes.Role)?.Value);
        Assert.Equal("project_token", result.Principal.FindFirst("token_type")?.Value);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WhenAuthorizationHeaderNotBearer_ReturnsNoResult()
    {
        const string rawToken = "sb_at_valid123";
        var httpContext = new DefaultHttpContext();
        httpContext.RequestAborted = TestContext.Current.CancellationToken;
        httpContext.Request.Headers.Authorization = $"CustomScheme {rawToken}";

        var scheme = new AuthenticationScheme(
            ProjectTokenAuthenticationOptions.Scheme,
            null,
            typeof(ProjectTokenAuthenticationHandler)
        );
        await _handler.InitializeAsync(scheme, httpContext);

        var result = await _handler.AuthenticateAsync();

        Assert.True(result.None);
        await _validateProjectTokenUseCase
            .DidNotReceive()
            .ValidateToken(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAuthenticateAsync_WhenValidQueryTokenOnLiveUpdatesPath_ReturnsSuccess()
    {
        const string rawToken = "sb_at_live123";
        var projectToken = new ProjectToken(
            new ProjectTokenId("tok-2"),
            ProjectId.FromValue("proj-2"),
            "hash-2",
            "SignalR token",
            new UserRole("viewer"),
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(7)
        );

        var httpContext = new DefaultHttpContext();
        httpContext.RequestAborted = TestContext.Current.CancellationToken;
        httpContext.Request.Path = "/api/projectLiveUpdates";
        httpContext.Request.QueryString = new QueryString($"?access_token={rawToken}");

        var scheme = new AuthenticationScheme(
            ProjectTokenAuthenticationOptions.Scheme,
            null,
            typeof(ProjectTokenAuthenticationHandler)
        );
        await _handler.InitializeAsync(scheme, httpContext);

        _validateProjectTokenUseCase
            .ValidateToken(rawToken, Arg.Any<CancellationToken>())
            .Returns(projectToken);

        var result = await _handler.AuthenticateAsync();

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Principal);
        Assert.Equal("tok-2", result.Principal.FindFirst("token_id")?.Value);
    }

    [Fact]
    public async Task HandleChallengeAsync_Sets401AndWwwAuthenticateHeader()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.RequestAborted = TestContext.Current.CancellationToken;
        var scheme = new AuthenticationScheme(
            ProjectTokenAuthenticationOptions.Scheme,
            null,
            typeof(ProjectTokenAuthenticationHandler)
        );
        await _handler.InitializeAsync(scheme, httpContext);

        await _handler.ChallengeAsync(new AuthenticationProperties());

        Assert.Equal(StatusCodes.Status401Unauthorized, httpContext.Response.StatusCode);
        Assert.Equal("Bearer error=\"invalid_token\"", httpContext.Response.Headers.WWWAuthenticate.ToString());
    }
}
