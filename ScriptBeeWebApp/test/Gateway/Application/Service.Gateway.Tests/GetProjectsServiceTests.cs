using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Ports.Permissions;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Gateway.Tests;

public class GetProjectsServiceTests
{
    private readonly IGetAllProjects _getAllProjects = Substitute.For<IGetAllProjects>();
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();

    private readonly IGetGlobalPermissions _getGlobalPermissions =
        Substitute.For<IGetGlobalPermissions>();

    private readonly IGetResourceRole _getResourceRole = Substitute.For<IGetResourceRole>();
    private readonly GetProjectsService _getProjectsService;

    private static readonly UserId UserId = new("user-id");
    private static readonly List<UserGroup> Groups = [new("group")];

    public GetProjectsServiceTests()
    {
        _getProjectsService = new GetProjectsService(
            _getAllProjects,
            _getProject,
            _getGlobalPermissions,
            _getResourceRole
        );
    }

    [Fact]
    public async Task GivenAdminUser_ShouldReturnAllProjects()
    {
        var project1 = BasicProjectDetails(ProjectId.Create("id-1"));
        var project2 = BasicProjectDetails(ProjectId.Create("id-2"));
        IEnumerable<ProjectDetails> allProjects = [project1, project2];
        var query = new GetAllProjectsQuery(UserId, Groups);

        _getGlobalPermissions
            .GetPermissions(UserId, Groups, Arg.Any<CancellationToken>())
            .Returns(["project:view_all"]);
        _getAllProjects
            .GetAll(TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(allProjects));

        var result = await _getProjectsService.GetAllProjects(
            query,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeEquivalentTo(allProjects);
        await _getResourceRole
            .DidNotReceive()
            .GetAccessibleProjectIds(
                Arg.Any<UserId>(),
                Arg.Any<List<UserGroup>>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GivenNonAdminUserWithAccessToSomeProjects_ShouldReturnOnlyAccessibleProjects()
    {
        var accessibleProjectId = ProjectId.Create("accessible");
        var inaccessibleProjectId = ProjectId.Create("inaccessible");
        var accessibleProject = BasicProjectDetails(accessibleProjectId);
        var inaccessibleProject = BasicProjectDetails(inaccessibleProjectId);
        IEnumerable<ProjectDetails> allProjects = [accessibleProject, inaccessibleProject];
        var query = new GetAllProjectsQuery(UserId, Groups);

        _getGlobalPermissions
            .GetPermissions(UserId, Groups, Arg.Any<CancellationToken>())
            .Returns([]);
        _getAllProjects
            .GetAll(TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(allProjects));
        _getResourceRole
            .GetAccessibleProjectIds(UserId, Groups, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<List<ProjectId>>([accessibleProjectId]));

        var result = await _getProjectsService.GetAllProjects(
            query,
            TestContext.Current.CancellationToken
        );

        result.Single().ShouldBe(accessibleProject);
    }

    [Fact]
    public async Task GivenNonAdminUserWithNoProjectAccess_ShouldReturnEmptyList()
    {
        var project = BasicProjectDetails(ProjectId.Create("id"));
        IEnumerable<ProjectDetails> allProjects = [project];
        var query = new GetAllProjectsQuery(UserId, Groups);

        _getGlobalPermissions
            .GetPermissions(UserId, Groups, Arg.Any<CancellationToken>())
            .Returns([]);
        _getAllProjects
            .GetAll(TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(allProjects));
        _getResourceRole
            .GetAccessibleProjectIds(UserId, Groups, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<List<ProjectId>>([]));

        var result = await _getProjectsService.GetAllProjects(
            query,
            TestContext.Current.CancellationToken
        );

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetProject()
    {
        var projectId = ProjectId.Create("id");
        var query = new GetProjectQuery(projectId);
        var expectedProjectDetails = BasicProjectDetails(projectId);
        _getProject
            .GetById(projectId, TestContext.Current.CancellationToken)
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    expectedProjectDetails
                )
            );

        var projectDetails = await _getProjectsService.GetProject(
            query,
            TestContext.Current.CancellationToken
        );

        projectDetails.ShouldBe(expectedProjectDetails);
    }

    [Fact]
    public async Task GivenNoProject_ShouldReturnProjectDoesNotExistsError()
    {
        var projectId = ProjectId.Create("id");
        var query = new GetProjectQuery(projectId);
        var expectedError = new ProjectDoesNotExistsError(projectId);
        _getProject
            .GetById(projectId, TestContext.Current.CancellationToken)
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(expectedError)
            );

        var error = await _getProjectsService.GetProject(
            query,
            TestContext.Current.CancellationToken
        );

        error.ShouldBe(expectedError);
    }
}
