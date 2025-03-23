using NSubstitute;
using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Files;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.Files;
using ScriptBee.UseCases.Project.Files;

namespace ScriptBee.Service.Project.Tests.Files;

public class UploadLoaderFilesServiceTest
{
    private readonly IGetProject _getProject = Substitute.For<IGetProject>();
    private readonly IFileModelService _fileModelService = Substitute.For<IFileModelService>();
    private readonly IGuidProvider _guidProvider = Substitute.For<IGuidProvider>();
    private readonly IUpdateProject _updateProject = Substitute.For<IUpdateProject>();

    private readonly UploadLoaderFilesService _uploadLoaderFilesService;

    public UploadLoaderFilesServiceTest()
    {
        _uploadLoaderFilesService = new UploadLoaderFilesService(
            _getProject,
            _fileModelService,
            _guidProvider,
            _updateProject
        );
    }

    [Fact]
    public async Task GiveNoProjectForProjectId_ExpectProjectDoesNotExistsError()
    {
        var projectId = ProjectId.FromValue("project-id");
        var command = new UploadLoaderFilesCommand(projectId, "loader-id", []);
        var projectDoesNotExistsError = new ProjectDoesNotExistsError(projectId);
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    projectDoesNotExistsError
                )
            );

        var result = await _uploadLoaderFilesService.Upload(command);

        result.AsT1.ShouldBe(projectDoesNotExistsError);
    }

    [Fact]
    public async Task GivenProject_ExpectPreviousSavedFilesToBeDeleted()
    {
        var projectId = ProjectId.FromValue("project-id");
        var command = new UploadLoaderFilesCommand(projectId, "loader-id", []);
        var projectDetails = new ProjectDetails(
            projectId,
            "name",
            DateTimeOffset.UtcNow,
            new Dictionary<string, List<FileData>>
            {
                {
                    "loader-id",
                    [
                        new FileData(new FileId("a6037f8e-575a-488b-91a8-3e5b0ddff9e1"), "file-1"),
                        new FileData(new FileId("38f4e81d-6039-4222-a8aa-af54fc6af648"), "file-2"),
                    ]
                },
                {
                    "other",
                    [new FileData(new FileId("0ecf0f1a-d66c-4a92-81c9-954f1d50b78b"), "file-3")]
                },
            }
        );
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        await _uploadLoaderFilesService.Upload(command);

        await _fileModelService
            .Received(1)
            .DeleteFilesAsync(
                Arg.Is<List<FileId>>(list =>
                    list.Count == 2
                    && list.Contains(new FileId("a6037f8e-575a-488b-91a8-3e5b0ddff9e1"))
                    && list.Contains(new FileId("38f4e81d-6039-4222-a8aa-af54fc6af648"))
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GivenFilesWithoutLength_ExpectFilesNotToBeUploaded()
    {
        var projectId = ProjectId.FromValue("project-id");
        var command = new UploadLoaderFilesCommand(
            projectId,
            "loader-id",
            [new UploadFileInformation("file-name", 0, new MemoryStream())]
        );
        var projectDetails = new ProjectDetails(
            projectId,
            "name",
            DateTimeOffset.UtcNow,
            new Dictionary<string, List<FileData>>()
        );
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );

        var result = await _uploadLoaderFilesService.Upload(command);

        result.AsT0.ShouldBeEmpty();
        await _fileModelService
            .Received(0)
            .UploadFileAsync(Arg.Any<FileId>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenFilesWithLength_ExpectFilesToBeUploaded()
    {
        var projectId = ProjectId.FromValue("project-id");
        var command = new UploadLoaderFilesCommand(
            projectId,
            "loader-id",
            [
                new UploadFileInformation("file-name-1", 2, new MemoryStream()),
                new UploadFileInformation("file-name-2", 0, new MemoryStream()),
                new UploadFileInformation("file-name-3", 5, new MemoryStream()),
            ]
        );
        var projectDetails = new ProjectDetails(
            projectId,
            "name",
            DateTimeOffset.UtcNow,
            new Dictionary<string, List<FileData>>()
        );
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

        var result = await _uploadLoaderFilesService.Upload(command);

        result
            .AsT0.ToList()
            .ShouldBe(
                [
                    new FileData(new FileId("825cba0f-1de8-42ef-8225-47400644f9e2"), "file-name-1"),
                    new FileData(new FileId("9e0ac246-fb26-461f-b77f-2c4fa64348b0"), "file-name-3"),
                ]
            );
        await _fileModelService
            .Received(1)
            .UploadFileAsync(
                new FileId("825cba0f-1de8-42ef-8225-47400644f9e2"),
                Arg.Any<Stream>(),
                Arg.Any<CancellationToken>()
            );
        await _fileModelService
            .Received(1)
            .UploadFileAsync(
                new FileId("9e0ac246-fb26-461f-b77f-2c4fa64348b0"),
                Arg.Any<Stream>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task GivenFilesWithLength_ExpectFilesToBeUpdatedInSavedFile()
    {
        var projectId = ProjectId.FromValue("project-id");
        var command = new UploadLoaderFilesCommand(
            projectId,
            "loader-id",
            [new UploadFileInformation("file", 2, new MemoryStream())]
        );
        var projectDetails = new ProjectDetails(
            projectId,
            "name",
            DateTimeOffset.UtcNow,
            new Dictionary<string, List<FileData>>
            {
                {
                    "loader-id",
                    [
                        new FileData(
                            new FileId("5420adf9-9c2c-426c-8574-05270496053c"),
                            "existing-file"
                        ),
                    ]
                },
                {
                    "other",
                    [new FileData(new FileId("8c88af6c-4d9b-4d56-9532-6478cd38ce0c"), "other-file")]
                },
            }
        );
        var updatedProjectDetails = projectDetails with
        {
            SavedFiles = new Dictionary<string, List<FileData>>
            {
                {
                    "loader-id",
                    [new FileData(new FileId("8b772016-480a-4dcf-868a-02804a7be0ff"), "file")]
                },
                {
                    "other",
                    [new FileData(new FileId("8c88af6c-4d9b-4d56-9532-6478cd38ce0c"), "other-file")]
                },
            },
        };
        _getProject
            .GetById(projectId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(projectDetails)
            );
        _guidProvider.NewGuid().Returns(Guid.Parse("8b772016-480a-4dcf-868a-02804a7be0ff"));

        await _uploadLoaderFilesService.Upload(command);

        await _updateProject
            .Received(1)
            .Update(
                Arg.Is<ProjectDetails>(details =>
                    MatchProjectDetails(details, updatedProjectDetails)
                ),
                Arg.Any<CancellationToken>()
            );
    }

    private static bool MatchProjectDetails(
        ProjectDetails details,
        ProjectDetails expectedProjectDetails
    )
    {
        var savedFiles = details.SavedFiles;
        var expectedSavedFiles = expectedProjectDetails.SavedFiles;
        return details.Id.Equals(expectedProjectDetails.Id)
            && details.Name.Equals(expectedProjectDetails.Name)
            && details.CreationDate.Equals(expectedProjectDetails.CreationDate)
            && savedFiles.Count == expectedSavedFiles.Count
            && MatchSavedFiles(savedFiles["loader-id"], expectedSavedFiles["loader-id"])
            && MatchSavedFiles(savedFiles["other"], expectedSavedFiles["other"]);
    }

    private static bool MatchSavedFiles(
        List<FileData> savedFiles,
        List<FileData> expectedSavedFiles
    )
    {
        return savedFiles.Count == expectedSavedFiles.Count
            && savedFiles.SequenceEqual(expectedSavedFiles);
    }
}
