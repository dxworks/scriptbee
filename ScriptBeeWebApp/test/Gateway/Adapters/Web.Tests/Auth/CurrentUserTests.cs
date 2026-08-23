using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using ScriptBee.Domain.Model.User;
using ScriptBee.UseCases.Gateway;
using ScriptBee.Web.Auth;
using ScriptBee.Web.Config;

namespace ScriptBee.Web.Tests.Auth;

public class CurrentUserTests
{
    [Fact]
    public async Task BindAsync_WhenUserIsNotAuthenticated_ReturnsNull()
    {
        var authConfig = new AuthenticationConfig
        {
            RequireHttpsMetadata = false,
            UserIdClaim = null,
            GroupsClaim = null,
        };
        var useCase = Substitute.For<IManageUsersUseCase>();
        var serviceProvider = new ServiceCollection()
            .AddSingleton(Options.Create(authConfig))
            .AddSingleton(useCase)
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity()),
            RequestServices = serviceProvider,
        };

        var result = await CurrentUser.BindAsync(httpContext);

        Assert.Null(result);
        await useCase
            .DidNotReceive()
            .GetUserId(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BindAsync_WhenDevelopmentMode_ReturnsCurrentUserWithEmptyId()
    {
        var authConfig = new AuthenticationConfig
        {
            AuthMode = "Development",
            RequireHttpsMetadata = false,
            UserIdClaim = null,
            GroupsClaim = null,
        };
        var useCase = Substitute.For<IManageUsersUseCase>();
        var serviceProvider = new ServiceCollection()
            .AddSingleton(Options.Create(authConfig))
            .AddSingleton(useCase)
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity()),
            RequestServices = serviceProvider,
        };

        var result = await CurrentUser.BindAsync(httpContext);

        Assert.NotNull(result);
        Assert.Equal(new UserId(string.Empty), result.Id);
        await useCase
            .DidNotReceive()
            .GetUserId(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
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
        var useCase = Substitute.For<IManageUsersUseCase>();
        useCase
            .GetUserId("user-456", "", Arg.Any<CancellationToken>())
            .Returns(new UserId("user-456"));

        var serviceProvider = new ServiceCollection()
            .AddSingleton(Options.Create(authConfig))
            .AddSingleton(useCase)
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
        await useCase.Received(1).GetUserId("user-456", "", Arg.Any<CancellationToken>());
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
        var useCase = Substitute.For<IManageUsersUseCase>();
        useCase
            .GetUserId("user-789", "", Arg.Any<CancellationToken>())
            .Returns(new UserId("user-789"));

        var serviceProvider = new ServiceCollection()
            .AddSingleton(Options.Create(authConfig))
            .AddSingleton(useCase)
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
        await useCase.Received(1).GetUserId("user-789", "", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BindAsync_WhenAuthenticated_AndUserIdClaimIsMissing_ReturnsNull()
    {
        var authConfig = new AuthenticationConfig
        {
            RequireHttpsMetadata = false,
            UserIdClaim = "missing-user-id",
            GroupsClaim = null,
        };
        var useCase = Substitute.For<IManageUsersUseCase>();

        var serviceProvider = new ServiceCollection()
            .AddSingleton(Options.Create(authConfig))
            .AddSingleton(useCase)
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider,
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, "fallback-user")],
                    "TestAuth"
                )
            ),
        };

        var result = await CurrentUser.BindAsync(httpContext);

        Assert.Null(result);
        await useCase
            .DidNotReceive()
            .GetUserId(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    public static TheoryData<string[], string> UserNameFallbackClaims =>
        new()
        {
            { ["custom-user-id|custom-user", $"{ClaimTypes.GivenName}|Alice"], "Alice" },
            { ["custom-user-id|custom-user", $"{ClaimTypes.Surname}|Smith"], "Smith" },
            { ["custom-user-id|custom-user", $"{ClaimTypes.Name}|Alice Smith"], "Alice Smith" },
            {
                ["custom-user-id|custom-user", $"{ClaimTypes.WindowsAccountName}|corp\\alice"],
                "corp\\alice"
            },
            {
                [
                    "custom-user-id|custom-user",
                    $"{ClaimTypes.GivenName}|Alice",
                    $"{ClaimTypes.Surname}|Smith",
                ],
                "Alice"
            },
            { ["custom-user-id|custom-user"], "" },
        };

    [Theory]
    [MemberData(nameof(UserNameFallbackClaims))]
    public async Task ExtractUserIdFromClaims_UsesExpectedUserNameFallback(
        string[] claimEntries,
        string expectedUserName
    )
    {
        // Arrange
        var claims = claimEntries
            .Select(entry =>
            {
                var separatorIndex = entry.IndexOf('|');

                return new Claim(entry[..separatorIndex], entry[(separatorIndex + 1)..]);
            })
            .ToArray();

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        var authConfig = new AuthenticationConfig
        {
            RequireHttpsMetadata = false,
            UserIdClaim = "custom-user-id",
            GroupsClaim = null,
        };
        var useCase = Substitute.For<IManageUsersUseCase>();
        useCase
            .GetUserId("custom-user", expectedUserName, Arg.Any<CancellationToken>())
            .Returns(new UserId("custom-user"));

        // Act
        var userId = await CurrentUser.ExtractUserIdFromClaims(
            claimsPrincipal,
            authConfig,
            useCase,
            CancellationToken.None
        );

        // Assert
        Assert.Equal(new UserId("custom-user"), userId);
        await useCase
            .Received(1)
            .GetUserId("custom-user", expectedUserName, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExtractUserIdFromClaims_WhenConfiguredClaimExists_ReturnsMatchedValue()
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
        var useCase = Substitute.For<IManageUsersUseCase>();
        useCase
            .GetUserId("custom-user", "", Arg.Any<CancellationToken>())
            .Returns(new UserId("custom-user"));

        var userId = await CurrentUser.ExtractUserIdFromClaims(
            claimsPrincipal,
            authConfig,
            useCase,
            CancellationToken.None
        );

        Assert.Equal(new UserId("custom-user"), userId);
        await useCase.Received(1).GetUserId("custom-user", "", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExtractUserIdFromClaims_WhenUserIdClaimIsNull_UsesNameIdentifier()
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
        var useCase = Substitute.For<IManageUsersUseCase>();
        useCase
            .GetUserId("fallback-user", "", Arg.Any<CancellationToken>())
            .Returns(new UserId("fallback-user"));

        var userId = await CurrentUser.ExtractUserIdFromClaims(
            claimsPrincipal,
            authConfig,
            useCase,
            CancellationToken.None
        );

        Assert.Equal(new UserId("fallback-user"), userId);
        await useCase.Received(1).GetUserId("fallback-user", "", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExtractUserIdFromClaims_WhenNoClaimsMatch_ReturnsNull()
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
        var useCase = Substitute.For<IManageUsersUseCase>();

        var userId = await CurrentUser.ExtractUserIdFromClaims(
            claimsPrincipal,
            authConfig,
            useCase,
            CancellationToken.None
        );

        Assert.Null(userId);
        await useCase
            .DidNotReceive()
            .GetUserId(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
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
