using NSubstitute;
using ScriptBee.Application.Model.Pagination;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.ProjectStructure;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.Tests.ProjectStructure;

public class GetProjectFilesServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();
    private readonly IGetScripts _getScripts = Substitute.For<IGetScripts>();

    private readonly GetProjectFilesService _getProjectFilesService;

    public GetProjectFilesServiceTest()
    {
        _getProjectFilesService = new GetProjectFilesService(_getProject, _getScripts);
    }

    [Fact]
    public async Task GivenProjectDoesNotExists_ShouldPropagateError()
    {
        var projectId = ProjectId.FromValue("id");
        var error = new ProjectDoesNotExistsError(projectId);
        _getProject.GetById(projectId, TestContext.Current.CancellationToken).Returns(error);

        var result = await _getProjectFilesService.GetAll(
            new GetProjectFilesQuery(projectId, null, 1, 2),
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(error);
    }

    [Fact]
    public async Task GivenNoParentId_ShouldListRootEntries()
    {
        var projectId = ProjectId.FromValue("id");
        var page = new Page<ProjectStructureEntry>([], 1, 2, 3);
        _getProject
            .GetById(projectId, TestContext.Current.CancellationToken)
            .Returns(ProjectDetailsFixture.BasicProjectDetails(projectId));
        _getScripts
            .ListRootEntries(projectId, 1, 2, TestContext.Current.CancellationToken)
            .Returns(page);

        var result = await _getProjectFilesService.GetAll(
            new GetProjectFilesQuery(projectId, null, 1, 2),
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(page);
    }

    [Fact]
    public async Task GivenParentId_WhenScriptDoesNotExists_ShouldPropagateError()
    {
        var projectId = ProjectId.FromValue("id");
        var parentId = new ScriptId(Guid.NewGuid());
        var error = new ScriptDoesNotExistsError(parentId);
        _getProject
            .GetById(projectId, TestContext.Current.CancellationToken)
            .Returns(ProjectDetailsFixture.BasicProjectDetails(projectId));
        _getScripts
            .ListEntries(projectId, parentId, 1, 2, TestContext.Current.CancellationToken)
            .Returns(error);

        var result = await _getProjectFilesService.GetAll(
            new GetProjectFilesQuery(projectId, parentId, 1, 2),
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(error);
    }

    [Fact]
    public async Task GivenParentId_ShouldListEntries()
    {
        var projectId = ProjectId.FromValue("id");
        var parentId = new ScriptId(Guid.NewGuid());
        var page = new Page<ProjectStructureEntry>([], 1, 2, 3);
        _getProject
            .GetById(projectId, TestContext.Current.CancellationToken)
            .Returns(ProjectDetailsFixture.BasicProjectDetails(projectId));
        _getScripts
            .ListEntries(projectId, parentId, 1, 2, TestContext.Current.CancellationToken)
            .Returns(page);

        var result = await _getProjectFilesService.GetAll(
            new GetProjectFilesQuery(projectId, parentId, 1, 2),
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(page);
    }
}
