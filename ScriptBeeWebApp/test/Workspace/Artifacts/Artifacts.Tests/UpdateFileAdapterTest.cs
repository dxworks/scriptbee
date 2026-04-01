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
    public async Task GetContentSuccessfully()
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

        var result = await _updateFileAdapter.UpdateScriptContent(
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
    public async Task GivenFileDoesNotExists_ThenFileDoesNotExistsErrorIsReturned()
    {
        const string pathToFileTxt = "file.txt";
        var projectId = ProjectId.FromValue("id");
        var pathToFileInSrcFolder = Path.Combine(_testTempFile, pathToFileTxt);
        _configFoldersService
            .GetPathToSrcFolder(projectId, pathToFileTxt)
            .Returns(pathToFileInSrcFolder);

        var result = await _updateFileAdapter.UpdateScriptContent(
            projectId,
            pathToFileTxt,
            "content",
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(new FileDoesNotExistsError(pathToFileTxt));
    }
}
