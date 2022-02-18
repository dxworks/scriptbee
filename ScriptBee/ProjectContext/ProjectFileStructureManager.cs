using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

    public void CreateFile(string projectId, string relativePath, string fileContent)
    {
        var filePath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, fileContent);
    }

    public string GetFileContent(string projectId, string relativePath)
    {
        var filePath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder, relativePath);
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }

        return null;
    }

    public Task<string> GetFileContentAsync(string projectId, string relativePath)
    {
        var filePath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder, relativePath);
        if (File.Exists(filePath))
        {
            return File.ReadAllTextAsync(filePath);
        }

        return Task.FromResult<>(null);
    }

    public FileStream GetFileContentStream(string projectId, string relativePath)
    {
        var filePath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder, relativePath);
        if (File.Exists(filePath))
        {
            return File.OpenRead(filePath);
        }

        return null;
    }

    public FileTreeNode GetSrcStructure(string projectId)
    {
        var srcPath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder);

        if (Directory.Exists(srcPath))
        {
            return GetFolderStructure(srcPath);
        }

        return null;
    }

    private FileTreeNode GetFolderStructure(string path)
    {
        if (File.Exists(path))
        {
            return new FileTreeNode(Path.GetFileName(path), null, path);
        }

        var children = new List<FileTreeNode>();

        foreach (var folderPath in Directory.GetDirectories(path))
        {
            children.Add(GetFolderStructure(folderPath));
        }

        foreach (var filePath in Directory.GetFiles(path))
        {
            children.Add(new FileTreeNode(Path.GetFileName(filePath), null, filePath));
        }

        return new FileTreeNode(Path.GetDirectoryName(path), children, path);
    }
}