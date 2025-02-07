using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Service.Projects;
using ScriptBee.Ports.Driven.Projects;
using ScriptBee.Ports.Driving.UseCases.Projects;
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
        var expectedProjectDetails = new ProjectDetails(ProjectId.FromValue("name"), "name");
        _createProject.CreateProject(expectedProjectDetails)
            .Returns(Task.FromResult<OneOf<Unit, ProjectIdAlreadyInUseError>>(new Unit()));

        var projectDetails = await _createProjectService.CreateProject(new CreateProjectCommand("name"));

        projectDetails.ShouldBe(expectedProjectDetails);
        await _createProject.Received(1).CreateProject(expectedProjectDetails);
    }

    [Fact]
    public async Task CreateProject_ShouldReturnProjectIdAlreadyInUse()
    {
        var projectId = ProjectId.FromValue("name");
        var expectedProjectDetails = new ProjectDetails(projectId, "name");
        var error = new ProjectIdAlreadyInUseError(projectId);
        _createProject.CreateProject(expectedProjectDetails)
            .Returns(
                Task.FromResult<OneOf<Unit, ProjectIdAlreadyInUseError>>(error));

        var projectDetails = await _createProjectService.CreateProject(new CreateProjectCommand("name"));

        projectDetails.ShouldBe(error);
        await _createProject.Received(1).CreateProject(expectedProjectDetails);
    }
}
