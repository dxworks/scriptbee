using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Project.Tests;

public class GetProjectsServiceTests
{
    private readonly IGetAllProjects _getAllProjects = Substitute.For<IGetAllProjects>();
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();
    private readonly GetProjectsService _getProjectsService;

    public GetProjectsServiceTests()
    {
        _getProjectsService = new GetProjectsService(_getAllProjects, _getProject);
    }

    [Fact]
    public async Task GetAllProjects()
    {
        var expectedProjectDetails = BasicProjectDetails(ProjectId.Create("id"));
        IEnumerable<ProjectDetails> projectDetails = new List<ProjectDetails>
        {
            expectedProjectDetails,
        };
        _getAllProjects
            .GetAll(TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(projectDetails));

        var projectDetailsList = await _getProjectsService.GetAllProjects(
            TestContext.Current.CancellationToken
        );

        projectDetailsList.ShouldBeEquivalentTo(
            new List<ProjectDetails> { expectedProjectDetails }
        );
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
