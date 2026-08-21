using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using ScriptBee.Web.Auth;

namespace ScriptBee.Web.Tests.Auth;

public class ExternalAuthorizationActionAuthorizationHandlerTests
{
    private readonly IAuthorizeExternally _authorizeExternally =
        Substitute.For<IAuthorizeExternally>();

    private readonly IExternalAuthorizationContextProvider _externalAuthorizationContextProvider =
        Substitute.For<IExternalAuthorizationContextProvider>();

    private readonly ExternalAuthorizationActionAuthorizationHandler _handler;

    public ExternalAuthorizationActionAuthorizationHandlerTests()
    {
        _handler = new ExternalAuthorizationActionAuthorizationHandler(
            _authorizeExternally,
            _externalAuthorizationContextProvider
        );
    }

    [Fact]
    public async Task GivenResourceNotHttpContext_ShouldNotSucceedRequirement()
    {
        // Arrange
        var requirement = new PermissionActionRequirement("TestAction");
        var context = new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(), null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
        await _externalAuthorizationContextProvider
            .DidNotReceive()
            .BuildRequestAsync(
                Arg.Any<HttpContext>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
        await _authorizeExternally
            .DidNotReceive()
            .IsAllowed(Arg.Any<ExternalAuthorizationRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenNotAllowedRequest_ShouldNotSucceedRequirement()
    {
        // Arrange
        var requirement = new PermissionActionRequirement("TestAction");
        var context = new AuthorizationHandlerContext(
            [requirement],
            new ClaimsPrincipal(),
            new DefaultHttpContext()
        );
        var request = new ExternalAuthorizationRequest();

        _externalAuthorizationContextProvider
            .BuildRequestAsync(
                Arg.Any<HttpContext>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(request);
        _authorizeExternally.IsAllowed(request, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task GivenAllowedRequest_ShouldSucceedRequirement()
    {
        // Arrange
        var requirement = new PermissionActionRequirement("TestAction");
        var context = new AuthorizationHandlerContext(
            [requirement],
            new ClaimsPrincipal(),
            new DefaultHttpContext()
        );
        var request = new ExternalAuthorizationRequest();

        _externalAuthorizationContextProvider
            .BuildRequestAsync(
                Arg.Any<HttpContext>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(request);
        _authorizeExternally.IsAllowed(request, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }
}
