using Microsoft.Extensions.Options;
using ScriptBee.Artifacts.Config;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts.Tests;

public class ConfigFoldersServiceTest
{
    private readonly ConfigFoldersService _configFoldersService = new(
        Options.Create(new UserFolderSettings { UserFolderPath = "user-folder-path" })
    );

    [Fact]
    public void GetPathToSrcFolder()
    {
        var pathToSrcFolder = _configFoldersService.GetPathToSrcFolder(
            ProjectId.FromValue("id"),
            "path/to/file.txt"
        );

        pathToSrcFolder
            .Replace("\\", "/")
            .ShouldBe($"{GetUserFolderPath()}/.scriptbee/projects/id/src/path/to/file.txt");
    }

    [Fact]
    public void GetAbsolutePathToSrcFolder_WithNonNullUserFolderPathOverride()
    {
        var pathToSrcFolder = _configFoldersService.GetAbsolutePathToSrcFolder(
            ProjectId.FromValue("id"),
            "path/to/file.txt"
        );

        pathToSrcFolder
            .Replace("\\", "/")
            .ShouldBe("user-folder-path/projects/id/src/path/to/file.txt");
    }

    [Fact]
    public void GetAbsolutePathToSrcFolder_WithNullUserFolderPathOverride()
    {
        var configFoldersService = new ConfigFoldersService(
            Options.Create(new UserFolderSettings { UserFolderPath = null })
        );

        var pathToSrcFolder = configFoldersService.GetAbsolutePathToSrcFolder(
            ProjectId.FromValue("id"),
            "path/to/file.txt"
        );

        pathToSrcFolder
            .Replace("\\", "/")
            .ShouldBe($"{GetUserFolderPath()}/.scriptbee/projects/id/src/path/to/file.txt");
    }

    private static string GetUserFolderPath() =>
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Replace("\\", "/");
}
