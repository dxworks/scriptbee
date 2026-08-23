using NSubstitute;
using OneOf;
using OneOf.Types;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Gateway.Tests;

public class CreateProjectServiceTests
{
    private readonly ICreateProject _createProject = Substitute.For<ICreateProject>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    private readonly IGetDefaultCreatorRole _getDefaultCreatorRole =
        Substitute.For<IGetDefaultCreatorRole>();

    private readonly ISetResourceRole _setResourceRole = Substitute.For<ISetResourceRole>();

    private readonly CreateProjectService _createProjectService;

    public CreateProjectServiceTests()
    {
        _createProjectService = new CreateProjectService(
            _createProject,
            _dateTimeProvider,
            _getDefaultCreatorRole,
            _setResourceRole
        );
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
            .Returns(Task.FromResult<OneOf<Success, ProjectIdAlreadyInUseError>>(new Success()));
        _dateTimeProvider.UtcNow().Returns(creationDate);

        var projectDetails = await _createProjectService.CreateProject(
            new CreateProjectCommand("id", "project", new UserId("user-id")),
            TestContext.Current.CancellationToken
        );

        MatchProjectDetails(projectDetails.AsT0, expectedProjectDetails).ShouldBeTrue();
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
    public async Task CreateProjectSuccessfully_ShouldAssignUserToProjectCreatorRole()
    {
        var creationDate = DateTimeOffset.Parse("2024-02-08");
        var userId = new UserId("user-id");
        var userRole = new UserRole("creator-role");
        _createProject
            .Create(Arg.Any<ProjectDetails>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<Success, ProjectIdAlreadyInUseError>>(new Success()));
        _dateTimeProvider.UtcNow().Returns(creationDate);
        _getDefaultCreatorRole.GetRole(Arg.Any<CancellationToken>()).Returns(userRole);

        await _createProjectService.CreateProject(
            new CreateProjectCommand("id", "project", userId),
            TestContext.Current.CancellationToken
        );

        await _setResourceRole
            .Received(1)
            .SetRoleForUser(
                userId,
                ProjectId.FromValue("id"),
                userRole,
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
            .Returns(Task.FromResult<OneOf<Success, ProjectIdAlreadyInUseError>>(error));
        _dateTimeProvider.UtcNow().Returns(creationDate);

        var projectDetails = await _createProjectService.CreateProject(
            new CreateProjectCommand("id", "project", new UserId("user-id")),
            TestContext.Current.CancellationToken
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
        await _getDefaultCreatorRole.DidNotReceive().GetRole(Arg.Any<CancellationToken>());
        await _setResourceRole
            .DidNotReceive()
            .SetRoleForUser(
                Arg.Any<UserId>(),
                Arg.Any<ProjectId>(),
                Arg.Any<UserRole>(),
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
