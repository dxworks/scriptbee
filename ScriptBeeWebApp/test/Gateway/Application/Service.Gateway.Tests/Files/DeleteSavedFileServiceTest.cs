using NSubstitute;
using OneOf;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Gateway.Files;
using ScriptBee.UseCases.Gateway.Files;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Gateway.Tests.Files;

public class DeleteSavedFileServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();
    private readonly IFileModelService _fileModelService = Substitute.For<IFileModelService>();
    private readonly IUpdateProject _updateProject = Substitute.For<IUpdateProject>();

    private readonly DeleteSavedFileService _deleteSavedFileService;

    public DeleteSavedFileServiceTest()
    {
        _deleteSavedFileService = new DeleteSavedFileService(
            _getProject,
            _fileModelService,
            _updateProject
        );
    }

    [Fact]
    public async Task GivenNoProjectForProjectId_ExpectProjectDoesNotExistsError()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var fileId = new FileId("a6037f8e-575a-488b-91a8-3e5b0ddff9e1");
        var command = new DeleteSavedFileCommand(projectId, fileId);
        var projectDoesNotExistsError = new ProjectDoesNotExistsError(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    projectDoesNotExistsError
                )
            );

        // Act
        var result = await _deleteSavedFileService.Delete(
            command,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.AsT1.ShouldBe(projectDoesNotExistsError);
    }

    [Fact]
    public async Task GivenFileNotInSavedFiles_ExpectProjectFileDoesNotExistsError()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var fileId = new FileId("a6037f8e-575a-488b-91a8-3e5b0ddff9e1");
        var command = new DeleteSavedFileCommand(projectId, fileId);
        var projectDetails = ProjectDetailsWithSavedFiles(projectId, []);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        // Act
        var result = await _deleteSavedFileService.Delete(
            command,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.AsT2.ShouldBe(new ProjectFileDoesNotExistsError(fileId));
    }

    [Fact]
    public async Task GivenFileInSavedFiles_ExpectFileDeletedFromSavedFilesLoadedFilesAndStorage()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var fileIdToDelete = new FileId("a6037f8e-575a-488b-91a8-3e5b0ddff9e1");
        var otherFileId = new FileId("38f4e81d-6039-4222-a8aa-af54fc6af648");
        var command = new DeleteSavedFileCommand(projectId, fileIdToDelete);
        var fileToDelete = new FileData(fileIdToDelete, "file-1");
        var otherFile = new FileData(otherFileId, "file-2");
        var projectDetails = new ProjectDetails(
            projectId,
            "project",
            DateTimeOffset.UtcNow,
            [fileToDelete, otherFile],
            new Dictionary<string, List<FileData>>
            {
                { "loader-1", [fileToDelete, otherFile] },
                { "loader-2", [fileToDelete] },
            },
            [],
            []
        );
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        // Act
        var result = await _deleteSavedFileService.Delete(
            command,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        await _fileModelService
            .Received(1)
            .DeleteFilesAsync(
                Arg.Is<IEnumerable<FileId>>(list => list.SequenceEqual(new[] { fileIdToDelete })),
                TestContext.Current.CancellationToken
            );
        await _updateProject
            .Received(1)
            .Update(
                Arg.Is<ProjectDetails>(details =>
                    details.SavedFiles.Count == 1
                    && details.SavedFiles[0].Equals(otherFile)
                    && details.LoadedFiles["loader-1"].Count == 1
                    && details.LoadedFiles["loader-1"][0].Equals(otherFile)
                    && details.LoadedFiles["loader-2"].Count == 0
                ),
                TestContext.Current.CancellationToken
            );
    }
}
