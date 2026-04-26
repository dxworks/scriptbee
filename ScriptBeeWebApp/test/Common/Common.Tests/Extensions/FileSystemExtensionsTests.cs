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

    [Fact]
    public void CopyTo_ThrowsDirectoryNotFoundException_WhenSourceDoesNotExist()
    {
        // Arrange
        var nonExistentDir = new DirectoryInfo(
            Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())
        );

        // Act & Assert
        Assert.Throws<DirectoryNotFoundException>(() => nonExistentDir.CopyTo("any_destination"));
    }

    [Fact]
    public void CopyTo_CopiesFiles_WhenNotRecursive()
    {
        // Arrange
        var sourcePath = fixture.CreateSubFolder("Source1");
        var destPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        File.WriteAllText(Path.Combine(sourcePath, "test.txt"), "hello");
        var subDir = Directory.CreateDirectory(Path.Combine(sourcePath, "Sub"));
        File.WriteAllText(Path.Combine(subDir.FullName, "hidden.txt"), "should not copy");

        var sourceDir = new DirectoryInfo(sourcePath);

        // Act
        sourceDir.CopyTo(destPath, recursive: false);

        // Assert
        Assert.True(File.Exists(Path.Combine(destPath, "test.txt")));
        Assert.False(Directory.Exists(Path.Combine(destPath, "Sub")));
    }

    [Fact]
    public void CopyTo_CopiesEverything_WhenRecursive()
    {
        // Arrange
        var sourcePath = fixture.CreateSubFolder("Source2");
        var destPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var subDirPath = Path.Combine(sourcePath, "Level1");
        Directory.CreateDirectory(subDirPath);
        File.WriteAllText(Path.Combine(sourcePath, "root.txt"), "root");
        File.WriteAllText(Path.Combine(subDirPath, "child.txt"), "child");

        var sourceDir = new DirectoryInfo(sourcePath);

        // Act
        sourceDir.CopyTo(destPath, recursive: true);

        // Assert
        Assert.True(File.Exists(Path.Combine(destPath, "root.txt")));
        Assert.True(File.Exists(Path.Combine(destPath, "Level1", "child.txt")));
    }

    [Fact]
    public void CopyTo_CreatesDestinationDirectory_IfItDoesNotExist()
    {
        // Arrange
        var sourcePath = fixture.CreateSubFolder("Source3");
        var destPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sourceDir = new DirectoryInfo(sourcePath);

        // Act
        sourceDir.CopyTo(destPath);

        // Assert
        Assert.True(Directory.Exists(destPath));
    }
}
