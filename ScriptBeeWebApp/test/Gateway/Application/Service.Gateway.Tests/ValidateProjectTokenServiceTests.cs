using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Service.Gateway.Tests;

public class ValidateProjectTokenServiceTests
{
    private readonly IGetProjectTokenByHash _getProjectTokenByHash =
        Substitute.For<IGetProjectTokenByHash>();

    private readonly ValidateProjectTokenService _service;

    public ValidateProjectTokenServiceTests()
    {
        _service = new ValidateProjectTokenService(_getProjectTokenByHash);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ValidateToken_WhenTokenIsNullOrEmpty_ReturnsNull(string? rawToken)
    {
        // Act
        var result = await _service.ValidateToken(rawToken!, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeNull();
        await _getProjectTokenByHash
            .DidNotReceive()
            .GetTokenByHash(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ValidateToken_WhenTokenDoesNotStartWithPrefix_ReturnsNull()
    {
        // Act
        var result = await _service.ValidateToken(
            "invalid_prefix_token",
            TestContext.Current.CancellationToken
        );

        // Assert
        result.ShouldBeNull();
        await _getProjectTokenByHash
            .DidNotReceive()
            .GetTokenByHash(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ValidateToken_WhenTokenNotFound_ReturnsNull()
    {
        // Arrange
        const string rawToken = "sb_at_nonexistent123";
        var expectedHash = ProjectToken.ComputeHash(rawToken);

        _getProjectTokenByHash
            .GetTokenByHash(expectedHash, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ProjectToken?>(null));

        // Act
        var result = await _service.ValidateToken(rawToken, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeNull();
        await _getProjectTokenByHash
            .Received(1)
            .GetTokenByHash(expectedHash, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ValidateToken_WhenTokenIsExpired_ReturnsNull()
    {
        // Arrange
        const string rawToken = "sb_at_expired123";
        var expectedHash = ProjectToken.ComputeHash(rawToken);
        var expiredToken = new ProjectToken(
            new ProjectTokenId("token-id"),
            ProjectId.FromValue("project-id"),
            expectedHash,
            "expired token",
            new UserRole("viewer"),
            DateTimeOffset.UtcNow.AddDays(-2),
            DateTimeOffset.UtcNow.AddMinutes(-5)
        );

        _getProjectTokenByHash
            .GetTokenByHash(expectedHash, Arg.Any<CancellationToken>())
            .Returns(expiredToken);

        // Act
        var result = await _service.ValidateToken(rawToken, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task ValidateToken_WhenTokenIsValid_ReturnsProjectToken()
    {
        // Arrange
        const string rawToken = "sb_at_valid123";
        var expectedHash = ProjectToken.ComputeHash(rawToken);
        var validToken = new ProjectToken(
            new ProjectTokenId("token-id"),
            ProjectId.FromValue("project-id"),
            expectedHash,
            "valid token",
            new UserRole("editor"),
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(7)
        );

        _getProjectTokenByHash
            .GetTokenByHash(expectedHash, Arg.Any<CancellationToken>())
            .Returns(validToken);

        // Act
        var result = await _service.ValidateToken(rawToken, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(validToken);
    }
}
