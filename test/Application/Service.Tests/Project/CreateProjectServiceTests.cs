using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Service.Project;
using ScriptBee.Ports.Driven.Project;
using ScriptBee.Ports.Driving.UseCases;
using ScriptBee.Ports.Driving.UseCases.Project;
using Shouldly;

namespace ScriptBee.Domain.Service.Tests.Project;

public class CreateProjectServiceTests
{
    private readonly ICreateProject _createProject = Substitute.For<ICreateProject>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly CreateProjectService _createProjectService;

    public CreateProjectServiceTests()
    {
        _createProjectService = new CreateProjectService(_createProject, _dateTimeProvider);
    }

    [Fact]
    public async Task CreateProjectSuccessfully()
    {
        var creationDate = DateTime.Parse("2024-02-08");
        var expectedProjectDetails = new ProjectDetails(ProjectId.Create("id"), "name", creationDate);
        _createProject.Create(expectedProjectDetails)
            .Returns(Task.FromResult<OneOf<Unit, ProjectIdAlreadyInUseError>>(new Unit()));
        _dateTimeProvider.UtcNow().Returns(creationDate);

        var projectDetails = await _createProjectService.CreateProject(new CreateProjectCommand("id", "name"));

        projectDetails.ShouldBe(expectedProjectDetails);
        await _createProject.Received(1).Create(expectedProjectDetails);
    }

    [Fact]
    public async Task CreateProject_ShouldReturnProjectIdAlreadyInUse()
    {
        var projectId = ProjectId.Create("id");
        var creationDate = DateTime.Parse("2024-02-08");
        var expectedProjectDetails = new ProjectDetails(projectId, "name", creationDate);
        var error = new ProjectIdAlreadyInUseError(projectId);
        _createProject.Create(expectedProjectDetails)
            .Returns(
                Task.FromResult<OneOf<Unit, ProjectIdAlreadyInUseError>>(error));
        _dateTimeProvider.UtcNow().Returns(creationDate);

        var projectDetails = await _createProjectService.CreateProject(new CreateProjectCommand("id", "name"));

        projectDetails.ShouldBe(error);
        await _createProject.Received(1).Create(expectedProjectDetails);
    }
}
