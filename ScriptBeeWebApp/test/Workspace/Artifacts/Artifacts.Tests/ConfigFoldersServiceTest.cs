using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts.Tests;

public class ConfigFoldersServiceTest
{
    private readonly ConfigFoldersService _configFoldersService = new();

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

    private static string GetUserFolderPath() =>
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Replace("\\", "/");
}
