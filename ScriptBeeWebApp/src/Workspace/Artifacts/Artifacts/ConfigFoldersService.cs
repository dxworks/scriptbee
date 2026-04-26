using ScriptBee.Domain.Model.Config;
using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Artifacts;

public class ConfigFoldersService : IConfigFoldersService
{
    public string GetPathToSrcFolder(ProjectId projectId, string path)
    {
        return Path.Combine(ConfigFolders.PathToProjects, projectId.ToString(), "src", path);
    }
}
