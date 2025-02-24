using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Service.Project;
using ScriptBee.Ports.Driven.Project;
using ScriptBee.Ports.Driving.UseCases.Project;
using Shouldly;

namespace ScriptBee.Domain.Service.Tests.Project;

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
        var expectedProjectDetails =
            new ProjectDetails(ProjectId.Create("id"), "name", DateTimeOffset.Parse("2024-02-08"));
        IEnumerable<ProjectDetails> projectDetails = new List<ProjectDetails> { expectedProjectDetails };
        _getAllProjects.GetAll()
            .Returns(Task.FromResult(projectDetails));

        var projectDetailsList = await _getProjectsService.GetAllProjects();

        projectDetailsList.ShouldBeEquivalentTo(new List<ProjectDetails> { expectedProjectDetails });
    }

    [Fact]
    public async Task GetProject()
    {
        var projectId = ProjectId.Create("id");
        var query = new GetProjectQuery(projectId);
        var expectedProjectDetails = new ProjectDetails(projectId, "name", DateTimeOffset.Parse("2024-02-08"));
        _getProject.GetById(projectId)
            .Returns(Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(expectedProjectDetails));

        var projectDetails = await _getProjectsService.GetProject(query);

        projectDetails.ShouldBe(expectedProjectDetails);
    }

    [Fact]
    public async Task GivenNoProject_ShouldReturnProjectDoesNotExistsError()
    {
        var projectId = ProjectId.Create("id");
        var query = new GetProjectQuery(projectId);
        var expectedError = new ProjectDoesNotExistsError(projectId);
        _getProject.GetById(projectId)
            .Returns(Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(expectedError));

        var error = await _getProjectsService.GetProject(query);

        error.ShouldBe(expectedError);
    }
}
