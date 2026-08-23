using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.Web.Auth;
using ScriptBee.Web.Config;

namespace ScriptBee.Web.Tests.Auth;

public class ExternalAuthorizationContextProviderTests
{
    private readonly IGetResourceRole _getResourceRole = Substitute.For<IGetResourceRole>();

    private readonly IOptions<AuthenticationConfig> _authConfigOptions = Substitute.For<
        IOptions<AuthenticationConfig>
    >();

    private readonly ExternalAuthorizationContextProvider _provider;

    public ExternalAuthorizationContextProviderTests()
    {
        _provider = new ExternalAuthorizationContextProvider(_getResourceRole, _authConfigOptions);
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
        Assert.Equal(userIdValue, result.Input.Subject.UserId);
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
        Assert.Equal(userIdValue, result.Input.Subject.UserId);
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
        Assert.Equal(userIdValue, result.Input.Subject.UserId);
        Assert.Equal(expectedGroups, result.Input.Subject.Groups);
        Assert.Equal(action, result.Input.Action);
        Assert.Equal("global", result.Input.Resource.Type);
        Assert.Null(result.Input.Resource.Id);
        Assert.Null(result.Input.Resource.Role);

        await _getResourceRole
            .DidNotReceiveWithAnyArgs()
            .GetRole(default!, null!, default!, TestContext.Current.CancellationToken);
    }
}
