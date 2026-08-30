using NSubstitute;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway.Tests;

public class ManageProjectTokensServiceTests
{
    private readonly ISecureRandomProvider _secureRandomProvider =
        Substitute.For<ISecureRandomProvider>();

    private readonly IGetAllProjectTokens _getAllProjectTokens =
        Substitute.For<IGetAllProjectTokens>();

    private readonly ICreateProjectToken _createProjectToken =
        Substitute.For<ICreateProjectToken>();

    private readonly IDeleteProjectToken _deleteProjectToken =
        Substitute.For<IDeleteProjectToken>();

    private readonly ManageProjectTokensService _service;

    public ManageProjectTokensServiceTests()
    {
        _service = new ManageProjectTokensService(
            _secureRandomProvider,
            _getAllProjectTokens,
            _createProjectToken,
            _deleteProjectToken
        );
    }

    [Fact]
    public async Task GetAllProjectTokens()
    {
        var projectId = ProjectId.FromValue("project-id");
        List<ProjectToken> projectTokens =
        [
            new(
                new ProjectTokenId("id"),
                projectId,
                "hashed-token",
                "description",
                new UserRole("role"),
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow
            ),
        ];
        _getAllProjectTokens
            .GetAllForProjectId(projectId, Arg.Any<CancellationToken>())
            .Returns(projectTokens);

        var tokens = await _service.GetProjectTokens(
            projectId,
            TestContext.Current.CancellationToken
        );

        tokens.ShouldBeEquivalentTo(projectTokens);
    }

    [Fact]
    public async Task CreateProjectToken()
    {
        var projectId = ProjectId.FromValue("project-id");
        var userRole = new UserRole("role");
        var expiresAt = DateTimeOffset.UtcNow;
        const string tokenHash = "8657B0297E515728040DB40B6E6A8C33F62B744A60159CC3CCF0BF913B458043";
        var command = new CreateProjectTokenCommand(projectId, "description", userRole, expiresAt);
        var token = new ProjectToken(
            new ProjectTokenId("id"),
            projectId,
            tokenHash,
            "description",
            userRole,
            DateTimeOffset.UtcNow,
            expiresAt
        );

        _secureRandomProvider.GetBytes(32).Returns([.. "ab"u8]);
        _createProjectToken
            .CreateToken(
                projectId,
                tokenHash,
                "description",
                userRole,
                expiresAt,
                Arg.Any<CancellationToken>()
            )
            .Returns(token);

        var result = await _service.CreateProjectToken(
            command,
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(new NewProjectTokenResult(token, "sb_at_YWI"));
    }

    [Fact]
    public async Task DeleteProjectToken()
    {
        var projectId = ProjectId.FromValue("project-id");
        var tokenId = new ProjectTokenId("id");

        await _service.DeleteProjectToken(
            projectId,
            tokenId,
            TestContext.Current.CancellationToken
        );

        await _deleteProjectToken
            .Received(1)
            .DeleteToken(projectId, tokenId, Arg.Any<CancellationToken>());
    }
}
