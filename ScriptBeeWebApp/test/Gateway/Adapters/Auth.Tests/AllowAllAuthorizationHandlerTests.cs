using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ScriptBee.Adapters.Auth.Dev;

namespace ScriptBee.Adapters.Auth.Tests;

file record DummyRequirement : IAuthorizationRequirement;

public class AllowAllAuthorizationHandlerTests
{
    private readonly AllowAllAuthorizationHandler _handler = new();

    [Fact]
    public async Task HandleAsync_ShouldSucceedAllPendingRequirements()
    {
        // Arrange
        var requirement1 = new DummyRequirement();
        var requirement2 = new DummyRequirement();

        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var requirements = new IAuthorizationRequirement[] { requirement1, requirement2 };

        var context = new AuthorizationHandlerContext(requirements, user, resource: null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
        Assert.False(context.HasFailed);
    }
}
