using NSubstitute;
using OneOf.Types;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts.Tests;

public sealed class UpdateFileAdapterTest : IDisposable
{
    private readonly IConfigFoldersService _configFoldersService =
        Substitute.For<IConfigFoldersService>();

    private readonly UpdateFileAdapter _updateFileAdapter;

    private readonly string _testTempFile = Path.Combine(
        Path.GetTempPath(),
        Guid.NewGuid().ToString()
    );

    public UpdateFileAdapterTest()
    {
        _updateFileAdapter = new UpdateFileAdapter(_configFoldersService);
        Directory.CreateDirectory(_testTempFile);
    }

    public void Dispose()
    {
        Directory.Delete(_testTempFile, true);
    }

    [Fact]
    public async Task GetContentSuccessfully_WhenUpdateContent()
    {
        const string pathToFileTxt = "file.txt";
        var projectId = ProjectId.FromValue("id");
        var pathToFileInSrcFolder = Path.Combine(_testTempFile, pathToFileTxt);
        _configFoldersService
            .GetPathToSrcFolder(projectId, pathToFileTxt)
            .Returns(pathToFileInSrcFolder);
        await File.WriteAllTextAsync(
            pathToFileInSrcFolder,
            "content",
            TestContext.Current.CancellationToken
        );

        var result = await _updateFileAdapter.UpdateContent(
            projectId,
            pathToFileTxt,
            "new-content",
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(new Success());
        var contentInFile = await File.ReadAllTextAsync(
            pathToFileInSrcFolder,
            TestContext.Current.CancellationToken
        );
        contentInFile.ShouldBe("new-content");
    }

    [Fact]
    public async Task GivenFileDoesNotExists_WhenUpdateContent_ThenFileDoesNotExistsErrorIsReturned()
    {
        const string pathToFileTxt = "file.txt";
        var projectId = ProjectId.FromValue("id");
        var pathToFileInSrcFolder = Path.Combine(_testTempFile, pathToFileTxt);
        _configFoldersService
            .GetPathToSrcFolder(projectId, pathToFileTxt)
            .Returns(pathToFileInSrcFolder);

        var result = await _updateFileAdapter.UpdateContent(
            projectId,
            pathToFileTxt,
            "content",
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(new FileDoesNotExistsError(pathToFileTxt));
    }

    [Fact]
    public void GivenOldPathFileDoesNotExists_WhenRenameFile_ThenFileDoesNotExistsErrorIsReturned()
    {
        const string oldPath = "file.txt";
        const string newPath = "file-new.txt";
        var projectId = ProjectId.FromValue("id");
        var pathToFileInSrcFolder = Path.Combine(_testTempFile, oldPath);
        _configFoldersService.GetPathToSrcFolder(projectId, oldPath).Returns(pathToFileInSrcFolder);

        var result = _updateFileAdapter.RenameFile(projectId, oldPath, newPath);

        result.AsT1.ShouldBe(new FileDoesNotExistsError(oldPath));
    }

    [Fact]
    public void GivenNewPathFileDoesExists_WhenRenameFile_ThenFileAlreadyExistsErrorIsReturned()
    {
        const string oldPath = "file.txt";
        const string newPath = "file-new.txt";
        var projectId = ProjectId.FromValue("id");
        var oldPathToFileInSrcFolder = Path.Combine(_testTempFile, oldPath);
        var newPathToFileInSrcFolder = Path.Combine(_testTempFile, newPath);
        _configFoldersService
            .GetPathToSrcFolder(projectId, oldPath)
            .Returns(oldPathToFileInSrcFolder);
        _configFoldersService
            .GetPathToSrcFolder(projectId, newPath)
            .Returns(newPathToFileInSrcFolder);
        File.WriteAllText(oldPathToFileInSrcFolder, "old-content");
        File.WriteAllText(newPathToFileInSrcFolder, "new-content");

        var result = _updateFileAdapter.RenameFile(projectId, oldPath, newPath);

        result.AsT2.ShouldBe(new FileAlreadyExistsError(newPath));
    }

    [Fact]
    public void GivenNewPath_WhenRenameFile_ThenRenameIsSuccessful()
    {
        const string oldPath = "file.txt";
        const string newPath = "file-new.txt";
        var projectId = ProjectId.FromValue("id");
        var oldPathToFileInSrcFolder = Path.Combine(_testTempFile, oldPath);
        var newPathToFileInSrcFolder = Path.Combine(_testTempFile, newPath);
        _configFoldersService
            .GetPathToSrcFolder(projectId, oldPath)
            .Returns(oldPathToFileInSrcFolder);
        _configFoldersService
            .GetPathToSrcFolder(projectId, newPath)
            .Returns(newPathToFileInSrcFolder);
        File.WriteAllText(oldPathToFileInSrcFolder, "content");

        var result = _updateFileAdapter.RenameFile(projectId, oldPath, newPath);

        result.AsT0.ShouldBe(new Success());
        File.Exists(oldPathToFileInSrcFolder).ShouldBeFalse();
        File.ReadAllText(newPathToFileInSrcFolder).ShouldBe("content");
    }
}
