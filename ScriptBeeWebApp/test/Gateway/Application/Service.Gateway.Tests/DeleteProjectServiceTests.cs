using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Permissions;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway;

namespace ScriptBee.Service.Gateway.Tests;

public class DeleteProjectServiceTests
{
    private readonly IDeleteProject _deleteProject = Substitute.For<IDeleteProject>();

    private readonly IRemoveProjectMember _removeProjectMember =
        Substitute.For<IRemoveProjectMember>();
    private readonly DeleteProjectService _deleteProjectService;

    public DeleteProjectServiceTests()
    {
        _deleteProjectService = new DeleteProjectService(_deleteProject, _removeProjectMember);
    }

    [Fact]
    public async Task DeleteProjectSuccessfully()
    {
        var projectId = ProjectId.Create("id");
        _deleteProject
            .Delete(projectId, TestContext.Current.CancellationToken)
            .Returns(Task.CompletedTask);

        await _deleteProjectService.DeleteProject(
            new DeleteProjectCommand(projectId),
            TestContext.Current.CancellationToken
        );

        await _deleteProject.Received(1).Delete(projectId, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ProjectMembersAreRemovedSuccessfully()
    {
        var projectId = ProjectId.Create("id");
        _deleteProject
            .Delete(projectId, TestContext.Current.CancellationToken)
            .Returns(Task.CompletedTask);

        await _deleteProjectService.DeleteProject(
            new DeleteProjectCommand(projectId),
            TestContext.Current.CancellationToken
        );

        await _removeProjectMember
            .Received(1)
            .RemoveAllProjectMembers(projectId, TestContext.Current.CancellationToken);
    }
}
