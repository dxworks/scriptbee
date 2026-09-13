using NSubstitute;
using OneOf;
using ScriptBee.Artifacts;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Gateway.Files;
using ScriptBee.UseCases.Gateway.Files;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Service.Gateway.Tests.Files;

public class UploadSavedFilesServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();
    private readonly IFileModelService _fileModelService = Substitute.For<IFileModelService>();
    private readonly IGuidProvider _guidProvider = Substitute.For<IGuidProvider>();
    private readonly IUpdateProject _updateProject = Substitute.For<IUpdateProject>();

    private readonly UploadSavedFilesService _uploadSavedFilesService;

    public UploadSavedFilesServiceTest()
    {
        _uploadSavedFilesService = new UploadSavedFilesService(
            _getProject,
            _fileModelService,
            _guidProvider,
            _updateProject
        );
    }

    [Fact]
    public async Task GivenNoProjectForProjectId_ExpectProjectDoesNotExistsError()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var command = new UploadSavedFilesCommand(projectId, []);
        var projectDoesNotExistsError = new ProjectDoesNotExistsError(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    projectDoesNotExistsError
                )
            );

        // Act
        var result = await _uploadSavedFilesService.Upload(
            command,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.AsT1.ShouldBe(projectDoesNotExistsError);
    }

    [Fact]
    public async Task GivenFilesWithoutLength_ExpectFilesNotToBeUploaded()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var command = new UploadSavedFilesCommand(
            projectId,
            [new UploadFileInformation("file-name", 0, new MemoryStream())]
        );
        var projectDetails = BasicProjectDetails(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        // Act
        var result = await _uploadSavedFilesService.Upload(
            command,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.AsT0.ShouldBeEmpty();
        await _fileModelService
            .Received(0)
            .UploadFileAsync<object>(
                Arg.Any<FileId>(),
                Arg.Any<Stream>(),
                null,
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GivenFilesWithLength_ExpectFilesToBeUploadedAndAppendedToSavedFiles()
    {
        // Arrange
        var projectId = ProjectId.FromValue("project-id");
        var command = new UploadSavedFilesCommand(
            projectId,
            [
                new UploadFileInformation("file-name-1", 2, new MemoryStream()),
                new UploadFileInformation("file-name-2", 0, new MemoryStream()),
                new UploadFileInformation("file-name-3", 5, new MemoryStream()),
            ]
        );
        var existingFile = new FileData(
            new FileId("a6037f8e-575a-488b-91a8-3e5b0ddff9e1"),
            "existing-file"
        );
        var projectDetails = ProjectDetailsWithSavedFiles(projectId, [existingFile]);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );
        _guidProvider
            .NewGuid()
            .Returns(
                Guid.Parse("825cba0f-1de8-42ef-8225-47400644f9e2"),
                Guid.Parse("9e0ac246-fb26-461f-b77f-2c4fa64348b0")
            );

        // Act
        var result = await _uploadSavedFilesService.Upload(
            command,
            TestContext.Current.CancellationToken
        );

        // Assert
        result
            .AsT0.ToList()
            .ShouldBe([
                new FileData(new FileId("825cba0f-1de8-42ef-8225-47400644f9e2"), "file-name-1"),
                new FileData(new FileId("9e0ac246-fb26-461f-b77f-2c4fa64348b0"), "file-name-3"),
            ]);
        await _fileModelService
            .Received(1)
            .UploadFileAsync<object>(
                new FileId("825cba0f-1de8-42ef-8225-47400644f9e2"),
                Arg.Any<Stream>(),
                null,
                Arg.Any<CancellationToken>()
            );
        await _fileModelService
            .Received(1)
            .UploadFileAsync<object>(
                new FileId("9e0ac246-fb26-461f-b77f-2c4fa64348b0"),
                Arg.Any<Stream>(),
                null,
                Arg.Any<CancellationToken>()
            );
        await _updateProject
            .Received(1)
            .Update(
                Arg.Is<ProjectDetails>(details =>
                    details.SavedFiles.Count == 3
                    && details.SavedFiles[0].Equals(existingFile)
                    && details
                        .SavedFiles[1]
                        .Id.Equals(new FileId("825cba0f-1de8-42ef-8225-47400644f9e2"))
                    && details
                        .SavedFiles[2]
                        .Id.Equals(new FileId("9e0ac246-fb26-461f-b77f-2c4fa64348b0"))
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
