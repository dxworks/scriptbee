using NSubstitute;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts.Tests;

public sealed class DeleteFileOrFolderAdapterTest : IDisposable
{
    private readonly IConfigFoldersService _configFoldersService =
        Substitute.For<IConfigFoldersService>();

    private readonly DeleteFileOrFolderAdapter _deleteFileOrFolderAdapter;

    private readonly string _testTempFile = Path.Combine(
        Path.GetTempPath(),
        Guid.NewGuid().ToString()
    );

    public DeleteFileOrFolderAdapterTest()
    {
        _deleteFileOrFolderAdapter = new DeleteFileOrFolderAdapter(_configFoldersService);
        Directory.CreateDirectory(_testTempFile);
    }

    public void Dispose()
    {
        Directory.Delete(_testTempFile, true);
    }

    [Fact]
    public void GivenPathDoesNotExists_ThenItThrowsNoException()
    {
        const string pathToFileTxt = "file.txt";
        var projectId = ProjectId.FromValue("id");
        _configFoldersService.GetPathToSrcFolder(projectId, pathToFileTxt).Returns("non-existing");

        var exception = Record.Exception(() =>
            _deleteFileOrFolderAdapter.Delete(projectId, pathToFileTxt)
        );

        exception.ShouldBeNull();
    }

    [Fact]
    public void GivenFileExists_ThenItIsDeleted()
    {
        const string pathToFileTxt = "file.txt";
        var projectId = ProjectId.FromValue("id");
        var pathToFileInSrcFolder = Path.Combine(_testTempFile, pathToFileTxt);
        _configFoldersService
            .GetPathToSrcFolder(projectId, pathToFileTxt)
            .Returns(pathToFileInSrcFolder);
        File.WriteAllText(pathToFileInSrcFolder, "content");

        _deleteFileOrFolderAdapter.Delete(projectId, pathToFileTxt);

        File.Exists(pathToFileInSrcFolder).ShouldBeFalse();
    }

    [Fact]
    public void GivenFolderExists_ThenItIsDeleted()
    {
        const string pathToFileTxt = "file.txt";
        var projectId = ProjectId.FromValue("id");
        var pathToFileInSrcFolder = Path.Combine(_testTempFile, pathToFileTxt);
        _configFoldersService
            .GetPathToSrcFolder(projectId, pathToFileTxt)
            .Returns(pathToFileInSrcFolder);
        Directory.CreateDirectory(pathToFileInSrcFolder);

        _deleteFileOrFolderAdapter.Delete(projectId, pathToFileTxt);

        Directory.Exists(pathToFileInSrcFolder).ShouldBeFalse();
    }

    [Fact]
    public void GivenNestedFolderExists_ThenItIsDeleted()
    {
        const string pathToFileTxt = "file.txt";
        var projectId = ProjectId.FromValue("id");
        var pathToFileInSrcFolder = Path.Combine(
            _testTempFile,
            "nested1",
            "nested2",
            pathToFileTxt
        );
        _configFoldersService
            .GetPathToSrcFolder(projectId, pathToFileTxt)
            .Returns(pathToFileInSrcFolder);
        Directory.CreateDirectory(pathToFileInSrcFolder);

        _deleteFileOrFolderAdapter.Delete(projectId, pathToFileTxt);

        Directory.Exists(pathToFileInSrcFolder).ShouldBeFalse();
    }
}
