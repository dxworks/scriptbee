using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Files;

namespace ScriptBee.Persistence.File.Tests;

public sealed class CreateFileAdapterTest : IDisposable
{
    private readonly IConfigFoldersService _configFoldersService =
        Substitute.For<IConfigFoldersService>();

    private readonly CreateFileAdapter _createFileAdapter;

    private readonly string _testTempFile = Path.Combine(
        Path.GetTempPath(),
        Guid.NewGuid().ToString()
    );

    public CreateFileAdapterTest()
    {
        _createFileAdapter = new CreateFileAdapter(_configFoldersService);
        Directory.CreateDirectory(_testTempFile);
    }

    public void Dispose()
    {
        Directory.Delete(_testTempFile, true);
    }

    [Fact]
    public async Task CreateFileSuccessfully()
    {
        const string pathToFileTxt = "path/to/file.txt";
        var projectId = ProjectId.FromValue("id");
        var pathToFileInSrcFolder = Path.Combine(_testTempFile, pathToFileTxt);
        _configFoldersService
            .GetPathToSrcFolder(projectId, pathToFileTxt)
            .Returns(pathToFileInSrcFolder);
        _configFoldersService
            .GetPathToUserFolder(pathToFileInSrcFolder)
            .Returns("path/to/user/folder/file.txt");

        var result = await _createFileAdapter.Create(
            projectId,
            pathToFileTxt,
            "content",
            TestContext.Current.CancellationToken
        );

        result.AsT0.ShouldBe(
            new CreateFileResult("file.txt", pathToFileTxt, "path/to/user/folder/file.txt")
        );
        (
            await System.IO.File.ReadAllTextAsync(
                pathToFileInSrcFolder,
                TestContext.Current.CancellationToken
            )
        ).ShouldBe("content");
    }

    [Fact]
    public async Task CreateFileAlreadyExists()
    {
        const string pathToFileTxt = "file.txt";
        var projectId = ProjectId.FromValue("id");
        var pathToFileInSrcFolder = Path.Combine(_testTempFile, pathToFileTxt);
        _configFoldersService
            .GetPathToSrcFolder(projectId, pathToFileTxt)
            .Returns(pathToFileInSrcFolder);
        await System.IO.File.WriteAllTextAsync(
            pathToFileInSrcFolder,
            "test",
            TestContext.Current.CancellationToken
        );

        var result = await _createFileAdapter.Create(
            projectId,
            pathToFileTxt,
            "content",
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(new FileAlreadyExistsError(pathToFileTxt));
    }
}
