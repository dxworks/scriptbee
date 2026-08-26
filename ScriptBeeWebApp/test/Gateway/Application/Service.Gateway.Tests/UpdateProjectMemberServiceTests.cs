using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway.Tests;

public class UpdateProjectMemberServiceTests
{
    private readonly ISetResourceRole _setResourceRole = Substitute.For<ISetResourceRole>();
    private readonly UpdateProjectMemberService _service;

    public UpdateProjectMemberServiceTests()
    {
        _service = new UpdateProjectMemberService(_setResourceRole);
    }

    [Fact]
    public async Task UpdateProjectMember_ShouldCallSetRoleForMemberWithCorrectArguments()
    {
        var projectId = ProjectId.FromValue("project-id");
        var command = new UpdateProjectMemberCommand(
            projectId,
            "user-a",
            "user",
            new UserRole("editor")
        );

        await _service.UpdateProjectMember(command, TestContext.Current.CancellationToken);

        await _setResourceRole
            .Received(1)
            .SetRoleForMember(
                "user-a",
                "user",
                projectId,
                new UserRole("editor"),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task UpdateProjectMember_ForGroup_ShouldCallSetRoleForMemberWithGroupType()
    {
        var projectId = ProjectId.FromValue("project-id");
        var command = new UpdateProjectMemberCommand(
            projectId,
            "dev-team",
            "group",
            new UserRole("viewer")
        );

        await _service.UpdateProjectMember(command, TestContext.Current.CancellationToken);

        await _setResourceRole
            .Received(1)
            .SetRoleForMember(
                "dev-team",
                "group",
                projectId,
                new UserRole("viewer"),
                Arg.Any<CancellationToken>()
            );
    }
}
