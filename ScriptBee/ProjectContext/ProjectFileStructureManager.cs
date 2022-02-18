using System.IO;
using ScriptBee.Config;

namespace ScriptBee.ProjectContext;

public class ProjectFileStructureManager : IProjectFileStructureManager
{
    public void CreateProjectFolderStructure(string projectId)
    {
        var projectPath = Path.Combine(ConfigFolders.PathToProjects, projectId);
        Directory.CreateDirectory(projectPath);
        Directory.CreateDirectory(Path.Combine(projectPath, ConfigFolders.SrcFolder));
        Directory.CreateDirectory(Path.Combine(projectPath, ConfigFolders.GeneratedFolder));
    }
}