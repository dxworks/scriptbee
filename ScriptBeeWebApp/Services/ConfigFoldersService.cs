using Microsoft.Extensions.Options;
using ScriptBee.Config;
using ScriptBee.Services.Config;

namespace ScriptBeeWebApp.Services;

public class ConfigFoldersService : IConfigFoldersService
{
    private readonly IOptions<UserFolderSettings> _userFolderSettingsOptions;

    public ConfigFoldersService(IOptions<UserFolderSettings> userFolderSettingsOptions)
    {
        _userFolderSettingsOptions = userFolderSettingsOptions;
    }

    public string GetAbsoluteFilePath(string projectId, string filePath)
    {
        var absolutePath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder, filePath);
        return GetPathToUserFolder(absolutePath);
    }

    public string GetProjectAbsolutePath(string projectId)
    {
        var projectPath = Path.Combine(ConfigFolders.PathToProjects, projectId);
        return GetPathToUserFolder(projectPath);
    }

    public string GetSrcRelativePath(string projectId, string filePath)
    {
        var srcPath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder);
        return Path.GetRelativePath(srcPath, filePath);
    }

    private string GetPathToUserFolder(string absolutePath)
    {
        if (string.IsNullOrEmpty(_userFolderSettingsOptions.Value.UserFolderPath))
        {
            return absolutePath;
        }

        var part = absolutePath.Replace("\\", "/")
            .Replace(ConfigFolders.PathToRoot.Replace("\\", "/"), "");

        return Path.Combine(_userFolderSettingsOptions.Value.UserFolderPath, part.TrimStart('\\', '/'))
            .Replace("\\", "/");
    }
}
