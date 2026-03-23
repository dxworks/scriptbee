using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.Config;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Persistence.File.Config;

namespace ScriptBee.Persistence.File;

public class ConfigFoldersService(IOptions<UserFolderSettings> userFolderSettingsOptions)
    : IConfigFoldersService
{
    public string GetPathToSrcFolder(ProjectId projectId, string path)
    {
        return Path.Combine(
            ConfigFolders.PathToProjects,
            projectId.ToString(),
            ConfigFolders.SrcFolder,
            path
        );
    }

    public string GetPathToUserFolder(string path)
    {
        if (string.IsNullOrEmpty(userFolderSettingsOptions.Value.UserFolderPath))
        {
            return path;
        }

        var part = path.Replace("\\", "/").Replace(ConfigFolders.PathToRoot.Replace("\\", "/"), "");

        return Path.Combine(
                userFolderSettingsOptions.Value.UserFolderPath,
                part.TrimStart('\\', '/')
            )
            .Replace("\\", "/");
    }
}
