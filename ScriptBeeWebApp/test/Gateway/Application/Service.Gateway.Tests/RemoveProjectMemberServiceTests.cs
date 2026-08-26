using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Permissions;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway.Tests;

public class RemoveProjectMemberServiceTests
{
    private readonly IRemoveProjectMember _removeProjectMember =
        Substitute.For<IRemoveProjectMember>();

    private readonly RemoveProjectMemberService _service;

    public RemoveProjectMemberServiceTests()
    {
        _service = new RemoveProjectMemberService(_removeProjectMember);
    }

    [Fact]
    public async Task RemoveProjectMember_ShouldCallPortWithCorrectArguments()
    {
        var projectId = ProjectId.FromValue("project-id");
        var command = new RemoveProjectMemberCommand(projectId, "user-a", "user");

        await _service.RemoveProjectMember(command, TestContext.Current.CancellationToken);

        await _removeProjectMember
            .Received(1)
            .RemoveProjectMember(projectId, "user-a", "user", Arg.Any<CancellationToken>());
    }
}
