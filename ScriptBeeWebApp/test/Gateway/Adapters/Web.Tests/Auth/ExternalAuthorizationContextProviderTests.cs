using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.Web.Auth;

namespace ScriptBee.Web.Tests.Auth;

public class ExternalAuthorizationContextProviderTests
{
    private readonly IResourceMemberService _resourceMemberService =
        Substitute.For<IResourceMemberService>();

    private readonly ExternalAuthorizationContextProvider _provider;

    public ExternalAuthorizationContextProviderTests()
    {
        _provider = new ExternalAuthorizationContextProvider(_resourceMemberService);
    }

    [Fact]
    public async Task BuildRequestAsync_WhenProjectIdExists_ReturnsProjectRequest()
    {
        const string userIdValue = "user-123";
        const string projectIdValue = "project-456";
        const string action = "read";
        const string expectedRoleValue = "project-admin";
        var expectedGroups = new[] { "admins", "reviewers" };
        var role = new UserRole(expectedRoleValue);

        _resourceMemberService
            .GetResourceRole(
                new UserId(userIdValue),
                Arg.Is<List<UserGroup>>(groups =>
                    groups.Count == 2
                    && groups[0] == new UserGroup("admins")
                    && groups[1] == new UserGroup("reviewers")
                ),
                ProjectId.FromValue(projectIdValue),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult(role));

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

        var routeData = new RouteData { Values = { ["projectId"] = projectIdValue } };
        httpContext.Features.Set<IRoutingFeature>(new RoutingFeature { RouteData = routeData });

        var result = await _provider.BuildRequestAsync(
            httpContext,
            action,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(userIdValue, result.Input.Subject.UserId);
        Assert.Equal(expectedGroups, result.Input.Subject.Groups);
        Assert.Equal(action, result.Input.Action);
        Assert.Equal("project", result.Input.Resource.Type);
        Assert.Equal(projectIdValue, result.Input.Resource.Id);
        Assert.Equal(expectedRoleValue, result.Input.Resource.Role);
    }

    [Fact]
    public async Task BuildRequestAsync_WhenProjectIdDoesNotExist_ReturnsGlobalRequest()
    {
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

        var result = await _provider.BuildRequestAsync(
            httpContext,
            action,
            TestContext.Current.CancellationToken
        );

        Assert.Equal(userIdValue, result.Input.Subject.UserId);
        Assert.Equal(expectedGroups, result.Input.Subject.Groups);
        Assert.Equal(action, result.Input.Action);
        Assert.Equal("global", result.Input.Resource.Type);
        Assert.Null(result.Input.Resource.Id);
        Assert.Null(result.Input.Resource.Role);

        await _resourceMemberService
            .DidNotReceiveWithAnyArgs()
            .GetResourceRole(default!, null!, default!, TestContext.Current.CancellationToken);
    }
}
