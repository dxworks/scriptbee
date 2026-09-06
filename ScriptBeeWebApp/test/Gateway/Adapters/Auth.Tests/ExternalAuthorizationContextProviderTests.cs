using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NSubstitute;
using OneOf;
using ScriptBee.Adapters.Auth.Config;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Adapters.Auth.Tests;

public class ExternalAuthorizationContextProviderTests
{
    private readonly IGetResourceRole _getResourceRole = Substitute.For<IGetResourceRole>();

    private readonly IOptions<AuthenticationConfig> _authConfigOptions = Substitute.For<
        IOptions<AuthenticationConfig>
    >();

    private readonly IManageUsersUseCase _manageUsersUseCase =
        Substitute.For<IManageUsersUseCase>();

    private readonly ExternalAuthorizationContextProvider _provider;

    public ExternalAuthorizationContextProviderTests()
    {
        _provider = new ExternalAuthorizationContextProvider(
            _getResourceRole,
            _authConfigOptions,
            _manageUsersUseCase
        );
    }

    [Fact]
    public async Task WhenProjectIdExists_ReturnsProjectRequest()
    {
        // Arrange
        const string userIdValue = "user-123";
        const string projectIdValue = "project-456";
        const string action = "read";
        const string expectedRoleValue = "project-admin";
        var role = new UserRole(expectedRoleValue);

        _manageUsersUseCase
            .GetUserId(userIdValue, "", Arg.Any<CancellationToken>())
            .Returns(new UserId(userIdValue));
        _getResourceRole
            .GetRole(
                new UserId(userIdValue),
                Arg.Is<List<UserGroup>>(groups => groups.Count == 0),
                ProjectId.FromValue(projectIdValue),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<UserRole?>(role));
        _authConfigOptions.Value.Returns(
            new AuthenticationConfig
            {
                RequireHttpsMetadata = false,
                UserIdClaim = null,
                GroupsClaim = null,
            }
        );

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, userIdValue),
                        new Claim("groups", "admins"),
                        new Claim("groups", "reviewers"),
                    ],
                    "TestAuth"
                )
            ),
        };

        var routeData = new RouteData { Values = { ["projectId"] = projectIdValue } };
        httpContext.Features.Set<IRoutingFeature>(new RoutingFeature { RouteData = routeData });

        // Act
        var result = await _provider.BuildRequestAsync(
            httpContext,
            action,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(userIdValue, result!.Input.Subject.UserId);
        Assert.Empty(result.Input.Subject.Groups);
        Assert.Equal(action, result.Input.Action);
        Assert.Equal("project", result.Input.Resource.Type);
        Assert.Equal(projectIdValue, result.Input.Resource.Id);
        Assert.Equal(expectedRoleValue, result.Input.Resource.Role);
    }

    [Fact]
    public async Task WhenProjectIdExists_AndClaimOverrideAreSet_ReturnsProjectRequest()
    {
        // Arrange
        const string userIdValue = "user-123";
        const string projectIdValue = "project-456";
        const string action = "read";
        const string expectedRoleValue = "project-admin";
        var role = new UserRole(expectedRoleValue);

        _manageUsersUseCase
            .GetUserId(userIdValue, "", Arg.Any<CancellationToken>())
            .Returns(new UserId(userIdValue));
        _getResourceRole
            .GetRole(
                new UserId(userIdValue),
                Arg.Is<List<UserGroup>>(groups =>
                    groups.Count == 1 && groups[0] == new UserGroup("admins")
                ),
                ProjectId.FromValue(projectIdValue),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<UserRole?>(role));
        _authConfigOptions.Value.Returns(
            new AuthenticationConfig
            {
                RequireHttpsMetadata = false,
                UserIdClaim = "other-sub",
                GroupsClaim = "other-groups",
            }
        );

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, "not-used"),
                        new Claim("other-sub", userIdValue),
                        new Claim("groups", "not-used"),
                        new Claim("other-groups", "admins"),
                    ],
                    "TestAuth"
                )
            ),
        };

        var routeData = new RouteData { Values = { ["projectId"] = projectIdValue } };
        httpContext.Features.Set<IRoutingFeature>(new RoutingFeature { RouteData = routeData });

        // Act
        var result = await _provider.BuildRequestAsync(
            httpContext,
            action,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(userIdValue, result!.Input.Subject.UserId);
        Assert.Equal(new[] { "admins" }, result.Input.Subject.Groups);
        Assert.Equal(action, result.Input.Action);
        Assert.Equal("project", result.Input.Resource.Type);
        Assert.Equal(projectIdValue, result.Input.Resource.Id);
        Assert.Equal(expectedRoleValue, result.Input.Resource.Role);
    }

    [Fact]
    public async Task WhenProjectIdDoesNotExist_ReturnsGlobalRequest()
    {
        // Arrange
        const string userIdValue = "user-456";
        const string action = "write";
        var expectedGroups = new[] { "admins", "ops" };

        _manageUsersUseCase
            .GetUserId(userIdValue, "", Arg.Any<CancellationToken>())
            .Returns(new UserId(userIdValue));
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, userIdValue),
                        new Claim("groups", expectedGroups[0]),
                        new Claim("groups", expectedGroups[1]),
                    ],
                    "TestAuth"
                )
            ),
        };
        _authConfigOptions.Value.Returns(
            new AuthenticationConfig
            {
                RequireHttpsMetadata = false,
                UserIdClaim = null,
                GroupsClaim = "groups",
            }
        );

        // Act
        var result = await _provider.BuildRequestAsync(
            httpContext,
            action,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(userIdValue, result!.Input.Subject.UserId);
        Assert.Equal(expectedGroups, result.Input.Subject.Groups);
        Assert.Equal(action, result.Input.Action);
        Assert.Equal("global", result.Input.Resource.Type);
        Assert.Null(result.Input.Resource.Id);
        Assert.Null(result.Input.Resource.Role);

        await _getResourceRole
            .DidNotReceiveWithAnyArgs()
            .GetRole(default!, null!, default!, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task WhenHubInvocationHasProjectId_ReturnsProjectRequest()
    {
        // Arrange
        const string userIdValue = "user-123";
        const string projectIdValue = "project-456";
        const string action = "project:live_updates";
        const string expectedRoleValue = "project-admin";
        var role = new UserRole(expectedRoleValue);

        _manageUsersUseCase
            .GetUserId(userIdValue, "", Arg.Any<CancellationToken>())
            .Returns(new UserId(userIdValue));
        _getResourceRole
            .GetRole(
                new UserId(userIdValue),
                Arg.Is<List<UserGroup>>(groups => groups.Count == 0),
                ProjectId.FromValue(projectIdValue),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<UserRole?>(role));
        _authConfigOptions.Value.Returns(
            new AuthenticationConfig
            {
                RequireHttpsMetadata = false,
                UserIdClaim = null,
                GroupsClaim = null,
            }
        );

        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userIdValue)], "TestAuth")
        );

        var hubCallerContext = Substitute.For<HubCallerContext>();
        hubCallerContext.User.Returns(claimsPrincipal);

        var hub = Substitute.For<Hub>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var hubMethod = typeof(TestHub).GetMethod(nameof(TestHub.JoinChannel))!;
        var hubInvocationContext = new HubInvocationContext(
            hubCallerContext,
            serviceProvider,
            hub,
            hubMethod,
            [projectIdValue, "scripts"]
        );

        // Act
        var result = await _provider.BuildRequestAsync(
            hubInvocationContext,
            action,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(userIdValue, result!.Input.Subject.UserId);
        Assert.Empty(result.Input.Subject.Groups);
        Assert.Equal(action, result.Input.Action);
        Assert.Equal("project", result.Input.Resource.Type);
        Assert.Equal(projectIdValue, result.Input.Resource.Id);
        Assert.Equal(expectedRoleValue, result.Input.Resource.Role);
    }

    [Fact]
    public async Task WhenHubInvocationDoesNotHaveProjectId_ReturnsGlobalRequest()
    {
        // Arrange
        const string userIdValue = "user-123";
        const string action = "project:live_updates";

        _manageUsersUseCase
            .GetUserId(userIdValue, "", Arg.Any<CancellationToken>())
            .Returns(new UserId(userIdValue));
        _authConfigOptions.Value.Returns(
            new AuthenticationConfig
            {
                RequireHttpsMetadata = false,
                UserIdClaim = null,
                GroupsClaim = null,
            }
        );

        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userIdValue)], "TestAuth")
        );

        var hubCallerContext = Substitute.For<HubCallerContext>();
        hubCallerContext.User.Returns(claimsPrincipal);

        var hub = Substitute.For<Hub>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var hubMethod = typeof(TestHub).GetMethod(nameof(TestHub.GlobalMethod))!;
        var hubInvocationContext = new HubInvocationContext(
            hubCallerContext,
            serviceProvider,
            hub,
            hubMethod,
            ["channel"]
        );

        // Act
        var result = await _provider.BuildRequestAsync(
            hubInvocationContext,
            action,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(userIdValue, result!.Input.Subject.UserId);
        Assert.Empty(result.Input.Subject.Groups);
        Assert.Equal(action, result.Input.Action);
        Assert.Equal("global", result.Input.Resource.Type);
        Assert.Null(result.Input.Resource.Id);
        Assert.Null(result.Input.Resource.Role);
    }

    [Fact]
    public async Task WhenPrincipalIsProjectToken_AndProjectIdMatchesRoute_ReturnsProjectRequestWithTokenRole()
    {
        const string tokenId = "token-1";
        const string projectId = "project-123";
        const string role = "editor";
        const string action = "script:view";

        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim("token_id", tokenId),
                    new Claim("project_id", projectId),
                    new Claim("role", role),
                    new Claim("token_type", "project_token"),
                ],
                "ProjectToken"
            )
        );

        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        httpContext.Request.RouteValues.Add("projectId", projectId);

        var result = await _provider.BuildRequestAsync(
            httpContext,
            action,
            TestContext.Current.CancellationToken
        );

        Assert.Equal($"project-token:{tokenId}", result!.Input.Subject.UserId);
        Assert.Empty(result.Input.Subject.Groups);
        Assert.Equal(action, result.Input.Action);
        Assert.Equal("project", result.Input.Resource.Type);
        Assert.Equal(projectId, result.Input.Resource.Id);
        Assert.Equal(role, result.Input.Resource.Role);
        await _manageUsersUseCase
            .DidNotReceive()
            .GetUserId(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _getResourceRole
            .DidNotReceive()
            .GetRole(
                Arg.Any<UserId>(),
                Arg.Any<List<UserGroup>>(),
                Arg.Any<OneOf<ProjectId>>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenPrincipalIsProjectToken_AndProjectIdDoesNotMatchRoute_ReturnsProjectRequestWithNullRole()
    {
        const string tokenId = "token-1";
        const string tokenProjectId = "project-123";
        const string requestedProjectId = "project-456";
        const string role = "editor";
        const string action = "script:view";

        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim("token_id", tokenId),
                    new Claim("project_id", tokenProjectId),
                    new Claim("role", role),
                    new Claim("token_type", "project_token"),
                ],
                "ProjectToken"
            )
        );

        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        httpContext.Request.RouteValues.Add("projectId", requestedProjectId);

        var result = await _provider.BuildRequestAsync(
            httpContext,
            action,
            TestContext.Current.CancellationToken
        );

        Assert.Equal($"project-token:{tokenId}", result!.Input.Subject.UserId);
        Assert.Empty(result.Input.Subject.Groups);
        Assert.Equal(action, result.Input.Action);
        Assert.Equal("project", result.Input.Resource.Type);
        Assert.Equal(requestedProjectId, result.Input.Resource.Id);
        Assert.Null(result.Input.Resource.Role);
    }

    [Fact]
    public async Task WhenPrincipalIsProjectToken_AndNoProjectIdInRoute_ReturnsGlobalRequestWithNullRole()
    {
        const string tokenId = "token-1";
        const string tokenProjectId = "project-123";
        const string role = "editor";
        const string action = "gateway_plugin:management";

        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim("token_id", tokenId),
                    new Claim("project_id", tokenProjectId),
                    new Claim("role", role),
                    new Claim("token_type", "project_token"),
                ],
                "ProjectToken"
            )
        );

        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        var result = await _provider.BuildRequestAsync(
            httpContext,
            action,
            TestContext.Current.CancellationToken
        );

        Assert.Equal($"project-token:{tokenId}", result!.Input.Subject.UserId);
        Assert.Empty(result.Input.Subject.Groups);
        Assert.Equal(action, result.Input.Action);
        Assert.Equal("global", result.Input.Resource.Type);
        Assert.Null(result.Input.Resource.Id);
        Assert.Null(result.Input.Resource.Role);
    }

    [Fact]
    public async Task WhenPrincipalIsProjectToken_AndHubInvocationProjectIdMatches_ReturnsProjectRequestWithTokenRole()
    {
        const string tokenId = "token-1";
        const string projectId = "project-123";
        const string role = "admin";
        const string action = "project:live_updates";

        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim("token_id", tokenId),
                    new Claim("project_id", projectId),
                    new Claim("role", role),
                    new Claim("token_type", "project_token"),
                ],
                "ProjectToken"
            )
        );

        var hubCallerContext = Substitute.For<HubCallerContext>();
        hubCallerContext.User.Returns(claimsPrincipal);

        var hub = Substitute.For<Hub>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var hubMethod = typeof(TestHub).GetMethod(nameof(TestHub.JoinChannel))!;
        var hubInvocationContext = new HubInvocationContext(
            hubCallerContext,
            serviceProvider,
            hub,
            hubMethod,
            [projectId, "channel"]
        );

        var result = await _provider.BuildRequestAsync(
            hubInvocationContext,
            action,
            TestContext.Current.CancellationToken
        );

        Assert.Equal($"project-token:{tokenId}", result!.Input.Subject.UserId);
        Assert.Empty(result.Input.Subject.Groups);
        Assert.Equal(action, result.Input.Action);
        Assert.Equal("project", result.Input.Resource.Type);
        Assert.Equal(projectId, result.Input.Resource.Id);
        Assert.Equal(role, result.Input.Resource.Role);
    }

    [Fact]
    public async Task WhenUserIdClaimNotFound_ReturnsNull()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _authConfigOptions.Value.Returns(
            new AuthenticationConfig
            {
                RequireHttpsMetadata = false,
                UserIdClaim = null,
                GroupsClaim = null,
            }
        );

        var result = await _provider.BuildRequestAsync(
            httpContext,
            "read",
            TestContext.Current.CancellationToken
        );

        Assert.Null(result);
    }

    private class TestHub : Hub
    {
        public Task JoinChannel(string projectId, string channelName) => Task.CompletedTask;

        public Task GlobalMethod(string channelName) => Task.CompletedTask;
    }
}
