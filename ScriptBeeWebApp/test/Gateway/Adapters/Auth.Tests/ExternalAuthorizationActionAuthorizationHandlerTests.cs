using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using ScriptBee.Adapters.Auth.Contracts;

namespace ScriptBee.Adapters.Auth.Tests;

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
        var request = GetExternalAuthorizationRequest();

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
        var request = GetExternalAuthorizationRequest();

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

    [Fact]
    public async Task GivenAllowedHubInvocationRequest_ShouldSucceedRequirement()
    {
        // Arrange
        var requirement = new PermissionActionRequirement("TestAction");
        var hubCallerContext = Substitute.For<HubCallerContext>();
        hubCallerContext.ConnectionAborted.Returns(TestContext.Current.CancellationToken);
        var hub = Substitute.For<Hub>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var hubMethod = typeof(TestHub).GetMethod(nameof(TestHub.TestMethod))!;
        var hubInvocationContext = new HubInvocationContext(
            hubCallerContext,
            serviceProvider,
            hub,
            hubMethod,
            ["project-123"]
        );
        var context = new AuthorizationHandlerContext(
            [requirement],
            new ClaimsPrincipal(),
            hubInvocationContext
        );
        var request = GetExternalAuthorizationRequest();

        _externalAuthorizationContextProvider
            .BuildRequestAsync(
                hubInvocationContext,
                "TestAction",
                TestContext.Current.CancellationToken
            )
            .Returns(request);
        _authorizeExternally
            .IsAllowed(request, TestContext.Current.CancellationToken)
            .Returns(true);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task GivenNotAllowedHubInvocationRequest_ShouldNotSucceedRequirement()
    {
        // Arrange
        var requirement = new PermissionActionRequirement("TestAction");
        var hubCallerContext = Substitute.For<HubCallerContext>();
        hubCallerContext.ConnectionAborted.Returns(TestContext.Current.CancellationToken);
        var hub = Substitute.For<Hub>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var hubMethod = typeof(TestHub).GetMethod(nameof(TestHub.TestMethod))!;
        var hubInvocationContext = new HubInvocationContext(
            hubCallerContext,
            serviceProvider,
            hub,
            hubMethod,
            ["project-123"]
        );
        var context = new AuthorizationHandlerContext(
            [requirement],
            new ClaimsPrincipal(),
            hubInvocationContext
        );
        var request = GetExternalAuthorizationRequest();

        _externalAuthorizationContextProvider
            .BuildRequestAsync(
                hubInvocationContext,
                "TestAction",
                TestContext.Current.CancellationToken
            )
            .Returns(request);
        _authorizeExternally
            .IsAllowed(request, TestContext.Current.CancellationToken)
            .Returns(false);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    private class TestHub : Hub
    {
        public Task TestMethod(string projectId) => Task.CompletedTask;
    }

    private static ExternalAuthorizationRequest GetExternalAuthorizationRequest()
    {
        return new ExternalAuthorizationRequest
        {
            Input = new ExternalAuthorizationRequestInput
            {
                Subject = new ExternalAuthorizationRequestSubject
                {
                    UserId = "user-id",
                    Groups = ["group"],
                },
                Action = "TestAction",
                Resource = new ExternalAuthorizationResource
                {
                    Type = "resource-type",
                    Id = "resource-id",
                    Role = "resource-role",
                },
            },
        };
    }
}
