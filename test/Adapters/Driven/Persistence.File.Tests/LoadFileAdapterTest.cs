using NSubstitute;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Files;

namespace ScriptBee.Persistence.File.Tests;

public sealed class LoadFileAdapterTest : IDisposable
{
    private readonly IConfigFoldersService _configFoldersService =
        Substitute.For<IConfigFoldersService>();

    private readonly LoadFileAdapter _loadFileAdapter;

    private readonly string _testTempFile = Path.Combine(
        Path.GetTempPath(),
        Guid.NewGuid().ToString()
    );

    public LoadFileAdapterTest()
    {
        _loadFileAdapter = new LoadFileAdapter(_configFoldersService);
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
        await System.IO.File.WriteAllTextAsync(pathToFileInSrcFolder, "content");

        var result = await _loadFileAdapter.GetScriptContent(projectId, pathToFileTxt);

        result.AsT0.ShouldBe("content");
    }

    [Fact]
    public async Task GivenFileDoesNotExists_GetContent_ThenFileDoesNotExistsErrorIsReturned()
    {
        const string pathToFileTxt = "file.txt";
        var projectId = ProjectId.FromValue("id");
        var pathToFileInSrcFolder = Path.Combine(_testTempFile, pathToFileTxt);
        _configFoldersService
            .GetPathToSrcFolder(projectId, pathToFileTxt)
            .Returns(pathToFileInSrcFolder);

        var result = await _loadFileAdapter.GetScriptContent(projectId, pathToFileTxt);

        result.AsT1.ShouldBe(new FileDoesNotExistsError(pathToFileTxt));
    }
}
