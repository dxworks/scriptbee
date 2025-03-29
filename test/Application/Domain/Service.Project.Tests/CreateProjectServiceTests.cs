using NSubstitute;
using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Project.Tests;

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
        var creationDate = DateTimeOffset.Parse("2024-02-08");
        var expectedProjectDetails = BasicProjectDetails(ProjectId.Create("id"), creationDate);
        _createProject
            .Create(
                Arg.Is<ProjectDetails>(details =>
                    MatchProjectDetails(details, expectedProjectDetails)
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<OneOf<Unit, ProjectIdAlreadyInUseError>>(new Unit()));
        _dateTimeProvider.UtcNow().Returns(creationDate);

        var projectDetails = await _createProjectService.CreateProject(
            new CreateProjectCommand("id", "project")
        );

        MatchProjectDetails(projectDetails.AsT0, expectedProjectDetails).ShouldBe(true);
        await _createProject
            .Received(1)
            .Create(
                Arg.Is<ProjectDetails>(details =>
                    MatchProjectDetails(details, expectedProjectDetails)
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task CreateProject_ShouldReturnProjectIdAlreadyInUse()
    {
        var projectId = ProjectId.Create("id");
        var creationDate = DateTimeOffset.Parse("2024-02-08");
        var expectedProjectDetails = BasicProjectDetails(projectId, creationDate);
        var error = new ProjectIdAlreadyInUseError(projectId);
        _createProject
            .Create(
                Arg.Is<ProjectDetails>(details =>
                    MatchProjectDetails(details, expectedProjectDetails)
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<OneOf<Unit, ProjectIdAlreadyInUseError>>(error));
        _dateTimeProvider.UtcNow().Returns(creationDate);

        var projectDetails = await _createProjectService.CreateProject(
            new CreateProjectCommand("id", "project")
        );

        projectDetails.ShouldBe(error);
        await _createProject
            .Received(1)
            .Create(
                Arg.Is<ProjectDetails>(details =>
                    MatchProjectDetails(details, expectedProjectDetails)
                ),
                Arg.Any<CancellationToken>()
            );
    }

    private static bool MatchProjectDetails(
        ProjectDetails details,
        ProjectDetails expectedProjectDetails
    )
    {
        return details.Id.Equals(expectedProjectDetails.Id)
            && details.Name.Equals(expectedProjectDetails.Name)
            && details.CreationDate.Equals(expectedProjectDetails.CreationDate)
            && details.SavedFiles.Count == expectedProjectDetails.SavedFiles.Count;
    }
}
