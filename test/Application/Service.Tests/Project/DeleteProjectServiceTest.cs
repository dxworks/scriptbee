using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Service.Project;
using ScriptBee.Ports.Driven.Project;
using ScriptBee.Ports.Driving.UseCases.Project;

namespace ScriptBee.Domain.Service.Tests.Project;

public class DeleteProjectServiceTest
{
    private readonly IDeleteProject _deleteProject = Substitute.For<IDeleteProject>();
    private readonly DeleteProjectService _deleteProjectService;

    public DeleteProjectServiceTest()
    {
        _deleteProjectService = new DeleteProjectService(_deleteProject);
    }

    [Fact]
    public async Task DeleteProjectSuccessfully()
    {
        var projectId = ProjectId.Create("id");
        _deleteProject.Delete(projectId)
            .Returns(Task.CompletedTask);

        await _deleteProjectService.DeleteProject(new DeleteProjectCommand(projectId));

        await _deleteProject.Received(1).Delete(projectId);
    }
}
