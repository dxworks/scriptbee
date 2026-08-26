using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Service.Gateway.Tests;

public class GetProjectMembersServiceTests
{
    private readonly IGetProjectMembers _getProjectMembers = Substitute.For<IGetProjectMembers>();
    private readonly GetProjectMembersService _service;

    public GetProjectMembersServiceTests()
    {
        _service = new GetProjectMembersService(_getProjectMembers);
    }

    [Fact]
    public async Task GetProjectMembers_ShouldReturnMembersFromPort()
    {
        var projectId = ProjectId.FromValue("project-id");
        var expectedMembers = new List<ProjectMember>
        {
            new("user-a", "user", new UserRole("owner")),
            new("team-b", "group", new UserRole("viewer")),
        };
        _getProjectMembers
            .GetProjectMembers(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedMembers));

        var result = await _service.GetProjectMembers(
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeEquivalentTo(expectedMembers);
    }

    [Fact]
    public async Task GetProjectMembers_WhenNoMembers_ShouldReturnEmptyList()
    {
        var projectId = ProjectId.FromValue("empty-project");
        _getProjectMembers
            .GetProjectMembers(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<ProjectMember>()));

        var result = await _service.GetProjectMembers(
            projectId,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeEmpty();
    }
}
