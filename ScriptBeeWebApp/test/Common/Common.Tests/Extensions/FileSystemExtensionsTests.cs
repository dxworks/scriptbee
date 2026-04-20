using ScriptBee.Common.Extensions;
using ScriptBee.Tests.Common;

namespace ScriptBee.Common.Tests.Extensions;

public class FileSystemExtensionsTests(TempDirFixture fixture) : IClassFixture<TempDirFixture>
{
    [Fact]
    public void GivenExistingDirectory_WhenDeleteIfExists_ThenDirectoryIsDeleted()
    {
        var path = fixture.CreateSubFolder("dir_to_delete");
        var directoryInfo = new DirectoryInfo(path);

        directoryInfo.DeleteIfExists();

        directoryInfo.Exists.ShouldBeFalse();
    }

    [Fact]
    public void GivenNonExistingDirectory_WhenDeleteIfExists_ThenNothingHappens()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var directoryInfo = new DirectoryInfo(path);

        directoryInfo.DeleteIfExists();

        directoryInfo.Exists.ShouldBeFalse();
    }

    [Fact]
    public void GivenExistingFile_WhenDeleteIfExists_ThenFileIsDeleted()
    {
        var path = Path.Combine(fixture.CreateSubFolder("file_delete"), "test.txt");
        File.WriteAllText(path, "test");
        var fileInfo = new FileInfo(path);

        fileInfo.DeleteIfExists();

        fileInfo.Exists.ShouldBeFalse();
    }

    [Fact]
    public void GivenNonExistingFile_WhenDeleteIfExists_ThenNothingHappens()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var fileInfo = new FileInfo(path);

        fileInfo.DeleteIfExists();

        fileInfo.Exists.ShouldBeFalse();
    }

    [Fact]
    public void GivenNonExistingDirectory_WhenEnsureDirectoryExists_ThenDirectoryIsCreated()
    {
        var path = Path.Combine(fixture.CreateSubFolder("ensure_dir"), "new_dir");

        path.EnsureDirectoryExists();

        Directory.Exists(path).ShouldBeTrue();
    }

    [Fact]
    public void GivenExistingDirectory_WhenEnsureDirectoryExists_ThenNothingHappens()
    {
        var path = fixture.CreateSubFolder("ensure_existing_dir");

        path.EnsureDirectoryExists();

        Directory.Exists(path).ShouldBeTrue();
    }
}
