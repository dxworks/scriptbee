using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Service.Project;
using ScriptBee.Ports.Driven.Project;
using ScriptBee.Ports.Driving.UseCases.Project;
using Shouldly;

namespace ScriptBee.Domain.Service.Tests;

public class CreateProjectServiceTests
{
    private readonly ICreateProject _createProject = Substitute.For<ICreateProject>();
    private readonly CreateProjectService _createProjectService;

    public CreateProjectServiceTests()
    {
        _createProjectService = new CreateProjectService(_createProject);
    }

    [Fact]
    public async Task CreateProjectSuccessfully()
    {
        var expectedProjectDetails = new ProjectDetails(ProjectId.Create("id"), "name");
        _createProject.CreateProject(expectedProjectDetails)
            .Returns(Task.FromResult<OneOf<Unit, ProjectIdAlreadyInUseError>>(new Unit()));

        var projectDetails = await _createProjectService.CreateProject(new CreateProjectCommand("id", "name"));

        projectDetails.ShouldBe(expectedProjectDetails);
        await _createProject.Received(1).CreateProject(expectedProjectDetails);
    }

    [Fact]
    public async Task CreateProject_ShouldReturnProjectIdAlreadyInUse()
    {
        var projectId = ProjectId.Create("id");
        var expectedProjectDetails = new ProjectDetails(projectId, "name");
        var error = new ProjectIdAlreadyInUseError(projectId);
        _createProject.CreateProject(expectedProjectDetails)
            .Returns(
                Task.FromResult<OneOf<Unit, ProjectIdAlreadyInUseError>>(error));

        var projectDetails = await _createProjectService.CreateProject(new CreateProjectCommand("id", "name"));

        projectDetails.ShouldBe(error);
        await _createProject.Received(1).CreateProject(expectedProjectDetails);
    }
}
