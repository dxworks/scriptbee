using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.User;
using ScriptBee.Web.Auth;
using ScriptBee.Web.Config;

namespace ScriptBee.Web.Tests.Auth;

public class CurrentUserTests
{
    [Fact]
    public async Task BindAsync_WhenUserIsNotAuthenticated_ReturnsNull()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton(
                Options.Create(
                    new AuthenticationConfig
                    {
                        RequireHttpsMetadata = false,
                        UserIdClaim = null,
                        GroupsClaim = null,
                    }
                )
            )
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity()),
            RequestServices = serviceProvider,
        };

        var result = await CurrentUser.BindAsync(httpContext);

        Assert.Null(result);
    }

    [Fact]
    public async Task BindAsync_WhenAuthenticated_UsesConfiguredUserIdClaim()
    {
        var authConfig = new AuthenticationConfig
        {
            RequireHttpsMetadata = false,
            UserIdClaim = "custom-user-id",
            GroupsClaim = "groups",
        };

        var serviceProvider = new ServiceCollection()
            .AddSingleton(Options.Create(authConfig))
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider,
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim("custom-user-id", "user-456"),
                        new Claim("groups", "admins"),
                        new Claim("groups", "reviewers"),
                    ],
                    "TestAuth"
                )
            ),
        };

        var result = await CurrentUser.BindAsync(httpContext);

        Assert.NotNull(result);
        Assert.Equal(new UserId("user-456"), result.Id);
    }

    [Fact]
    public async Task BindAsync_WhenAuthenticated_UsesDefaultNameIdentifierClaim()
    {
        var authConfig = new AuthenticationConfig
        {
            RequireHttpsMetadata = false,
            UserIdClaim = null,
            GroupsClaim = null,
        };

        var serviceProvider = new ServiceCollection()
            .AddSingleton(Options.Create(authConfig))
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider,
            User = new ClaimsPrincipal(
                new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "user-789")], "TestAuth")
            ),
        };

        var result = await CurrentUser.BindAsync(httpContext);

        Assert.NotNull(result);
        Assert.Equal(new UserId("user-789"), result.Id);
    }

    [Fact]
    public void ExtractUserIdFromClaims_WhenConfiguredClaimExists_ReturnsMatchedValue()
    {
        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, "ignored-user"),
                    new Claim("custom-user-id", "custom-user"),
                ],
                "TestAuth"
            )
        );
        var authConfig = new AuthenticationConfig
        {
            RequireHttpsMetadata = false,
            UserIdClaim = "custom-user-id",
            GroupsClaim = null,
        };

        var userId = CurrentUser.ExtractUserIdFromClaims(claimsPrincipal, authConfig);

        Assert.Equal(new UserId("custom-user"), userId);
    }

    [Fact]
    public void ExtractUserIdFromClaims_WhenUserIdClaimIsNull_UsesNameIdentifier()
    {
        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "fallback-user")], "TestAuth")
        );
        var authConfig = new AuthenticationConfig
        {
            RequireHttpsMetadata = false,
            UserIdClaim = null,
            GroupsClaim = null,
        };

        var userId = CurrentUser.ExtractUserIdFromClaims(claimsPrincipal, authConfig);

        Assert.Equal(new UserId("fallback-user"), userId);
    }

    [Fact]
    public void ExtractUserIdFromClaims_WhenNoClaimsMatch_ReturnsEmptyUserId()
    {
        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim("other-claim", "whatever")], "TestAuth")
        );
        var authConfig = new AuthenticationConfig
        {
            RequireHttpsMetadata = false,
            UserIdClaim = "missing-user-id",
            GroupsClaim = null,
        };

        var userId = CurrentUser.ExtractUserIdFromClaims(claimsPrincipal, authConfig);

        Assert.Equal(new UserId(string.Empty), userId);
    }

    [Fact]
    public void ExtractGroupsFromClaims_WhenGroupsClaimIsNull_ReturnsEmptyList()
    {
        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                [new Claim("groups", "admins"), new Claim("groups", "reviewers")],
                "TestAuth"
            )
        );
        var authConfig = new AuthenticationConfig
        {
            RequireHttpsMetadata = false,
            UserIdClaim = null,
            GroupsClaim = null,
        };

        var groups = CurrentUser.ExtractGroupsFromClaims(claimsPrincipal, authConfig);

        Assert.Empty(groups);
    }

    [Fact]
    public void ExtractGroupsFromClaims_WhenGroupsClaimExists_ReturnsMappedGroups()
    {
        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim("groups", "admins"),
                    new Claim("groups", "reviewers"),
                    new Claim("other-groups", "ignored"),
                ],
                "TestAuth"
            )
        );
        var authConfig = new AuthenticationConfig
        {
            RequireHttpsMetadata = false,
            UserIdClaim = null,
            GroupsClaim = "groups",
        };

        var groups = CurrentUser.ExtractGroupsFromClaims(claimsPrincipal, authConfig);

        Assert.Equal([new UserGroup("admins"), new UserGroup("reviewers")], groups);
    }
}
