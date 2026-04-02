using NSubstitute;
using OneOf;
using OneOf.Types;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.ProjectStructure;
using ScriptBee.UseCases.Project.ProjectStructure;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Project.Tests.ProjectStructure;

public class DeleteProjectFilesServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();
    private readonly IDeleteScript _deleteScript = Substitute.For<IDeleteScript>();

    private readonly IDeleteFileOrFolder _deleteFileOrFolder =
        Substitute.For<IDeleteFileOrFolder>();

    private readonly DeleteProjectFilesService _deleteProjectFilesService;

    public DeleteProjectFilesServiceTest()
    {
        _deleteProjectFilesService = new DeleteProjectFilesService(
            _getProject,
            _deleteScript,
            _deleteFileOrFolder
        );
    }

    [Fact]
    public async Task ProjectDoesNotExists()
    {
        var projectId = ProjectId.FromValue("id");
        var error = new ProjectDoesNotExistsError(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(error));

        var result = await _deleteProjectFilesService.Delete(
            new DeleteFileCommand(projectId, new ScriptId(Guid.NewGuid())),
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(error);
    }

    [Fact]
    public async Task DeleteScriptEntryAndFileOrFolder()
    {
        // Arrange
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        var entry = new ScriptFolder(scriptId, projectId, new ProjectStructureFile("path"), []);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _deleteScript.Delete(scriptId, Arg.Any<CancellationToken>()).Returns(entry);

        // Act
        var result = await _deleteProjectFilesService.Delete(
            new DeleteFileCommand(projectId, scriptId),
            TestContext.Current.CancellationToken
        );

        // Assert
        result.ShouldBe(new Success());
        _deleteFileOrFolder.Received(1).Delete(projectId, "path");
    }

    [Fact]
    public async Task DeleteScriptReturnsNull_EntryAndFileOrFolderDeletionIsNotNeeded()
    {
        // Arrange
        var projectId = ProjectId.FromValue("id");
        var scriptId = new ScriptId(Guid.NewGuid());
        ProjectStructureEntry? entry = null;
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(projectId)
                )
            );
        _deleteScript.Delete(scriptId, Arg.Any<CancellationToken>()).Returns(entry);

        // Act
        var result = await _deleteProjectFilesService.Delete(
            new DeleteFileCommand(projectId, scriptId),
            TestContext.Current.CancellationToken
        );

        // Assert
        result.ShouldBe(new Success());
        _deleteFileOrFolder.Received(0).Delete(Arg.Any<ProjectId>(), Arg.Any<string>());
    }
}
