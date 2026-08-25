using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Adapters.Auth.Contracts;

namespace ScriptBee.Adapters.Auth.Tests;

public class SignalRHubAuthorizationTests
{
    private readonly IAuthorizeExternally _authorizeExternally =
        Substitute.For<IAuthorizeExternally>();
    private readonly IExternalAuthorizationContextProvider _contextProvider =
        Substitute.For<IExternalAuthorizationContextProvider>();
    private readonly IAuthorizationService _authorizationService;

    public SignalRHubAuthorizationTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorizationCore();
        services.AddSingleton<
            IAuthorizationPolicyProvider,
            PermissionActionAuthorizationPolicyProvider
        >();
        services.AddSingleton<
            IAuthorizationHandler,
            ExternalAuthorizationActionAuthorizationHandler
        >();
        services.AddSingleton(_authorizeExternally);
        services.AddSingleton(_contextProvider);

        var serviceProvider = services.BuildServiceProvider();
        _authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
    }

    [Fact]
    public async Task GivenSignalRHubInvocation_WhenUserIsAllowed_AuthorizationShouldSucceed()
    {
        // Arrange
        const string projectId = "proj-123";
        const string action = "project:live_updates";

        var hubCallerContext = Substitute.For<HubCallerContext>();
        hubCallerContext.ConnectionAborted.Returns(TestContext.Current.CancellationToken);

        var hub = Substitute.For<Hub>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var hubMethod = typeof(TestSignalRHub).GetMethod(nameof(TestSignalRHub.JoinChannel))!;
        var hubInvocationContext = new HubInvocationContext(
            hubCallerContext,
            serviceProvider,
            hub,
            hubMethod,
            [projectId, "scripts"]
        );

        var user = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "user-1")], "TestAuth")
        );

        var authRequest = new ExternalAuthorizationRequest
        {
            Input = new ExternalAuthorizationRequestInput
            {
                Subject = new ExternalAuthorizationRequestSubject
                {
                    UserId = "user-1",
                    Groups = [],
                },
                Action = action,
                Resource = new ExternalAuthorizationResource
                {
                    Type = "project",
                    Id = projectId,
                    Role = "Editor",
                },
            },
        };

        _contextProvider
            .BuildRequestAsync(hubInvocationContext, action, TestContext.Current.CancellationToken)
            .Returns(authRequest);
        _authorizeExternally
            .IsAllowed(authRequest, TestContext.Current.CancellationToken)
            .Returns(true);

        // Act
        var result = await _authorizationService.AuthorizeAsync(user, hubInvocationContext, action);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task GivenSignalRHubInvocation_WhenUserIsNotAllowed_AuthorizationShouldFail()
    {
        // Arrange
        const string projectId = "proj-123";
        const string action = "project:live_updates";

        var hubCallerContext = Substitute.For<HubCallerContext>();
        hubCallerContext.ConnectionAborted.Returns(TestContext.Current.CancellationToken);

        var hub = Substitute.For<Hub>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var hubMethod = typeof(TestSignalRHub).GetMethod(nameof(TestSignalRHub.JoinChannel))!;
        var hubInvocationContext = new HubInvocationContext(
            hubCallerContext,
            serviceProvider,
            hub,
            hubMethod,
            [projectId, "scripts"]
        );

        var user = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "user-1")], "TestAuth")
        );

        var authRequest = new ExternalAuthorizationRequest
        {
            Input = new ExternalAuthorizationRequestInput
            {
                Subject = new ExternalAuthorizationRequestSubject
                {
                    UserId = "user-1",
                    Groups = [],
                },
                Action = action,
                Resource = new ExternalAuthorizationResource
                {
                    Type = "project",
                    Id = projectId,
                    Role = "Viewer",
                },
            },
        };

        _contextProvider
            .BuildRequestAsync(hubInvocationContext, action, TestContext.Current.CancellationToken)
            .Returns(authRequest);
        _authorizeExternally
            .IsAllowed(authRequest, TestContext.Current.CancellationToken)
            .Returns(false);

        // Act
        var result = await _authorizationService.AuthorizeAsync(user, hubInvocationContext, action);

        // Assert
        Assert.False(result.Succeeded);
    }

    private class TestSignalRHub : Hub
    {
        [AuthorizeAction("project:live_updates")]
        public Task JoinChannel(string projectId, string channelName) => Task.CompletedTask;
    }
}
