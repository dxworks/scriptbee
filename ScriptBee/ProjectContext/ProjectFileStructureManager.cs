using System;
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
        var filePath = Path.Combine(ConfigFolders.PathToProjects, projectId, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, fileContent);
    }

    public void CreateSrcFile(string projectId, string relativePath, string fileContent)
    {
        var filePath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, fileContent);
    }

    public bool FileExists(string projectId, string relativePath)
    {
        var filePath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder, relativePath);
        return File.Exists(filePath);
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

    public async Task<string?> GetFileContentAsync(string projectId, string relativePath)
    {
        var filePath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder, relativePath);
        if (File.Exists(filePath))
        {
            return await File.ReadAllTextAsync(filePath);
        }

        return null;
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

    public FileTreeNode? GetSrcStructure(string projectId)
    {
        var srcPath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder);

        if (Directory.Exists(srcPath))
        {
            return GetFolderStructure(srcPath, srcPath);
        }

        return null;
    }

    public string GetAbsoluteFilePath(string projectId, string filePath)
    {
        var absolutePath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder, filePath);

        var envUserFolderPath = Environment.GetEnvironmentVariable("USER_FOLDER_PATH");
        if (!string.IsNullOrEmpty(envUserFolderPath))
        {
            var part = absolutePath.Replace(ConfigFolders.PathToUserFolder, "");
            return Path.Combine(envUserFolderPath, part.TrimStart('\\', '/'));
        }

        return absolutePath;
    }

    public void DeleteFolder(string projectId, string pathToFolder)
    {
        var folderAbsolutePath = Path.Combine(ConfigFolders.PathToProjects, projectId, pathToFolder);

        if (Directory.Exists(folderAbsolutePath))
        {
            Directory.Delete(folderAbsolutePath, true);
        }
    }

    private FileTreeNode GetFolderStructure(string path, string srcPath)
    {
        if (File.Exists(path))
        {
            return new FileTreeNode(Path.GetFileName(path), null, path, Path.GetRelativePath(srcPath, path));
        }

        var children = new List<FileTreeNode>();

        foreach (var folderPath in Directory.GetDirectories(path))
        {
            children.Add(GetFolderStructure(folderPath, srcPath));
        }

        foreach (var filePath in Directory.GetFiles(path))
        {
            children.Add(new FileTreeNode(Path.GetFileName(filePath), null, filePath,
                Path.GetRelativePath(srcPath, filePath)));
        }

        return new FileTreeNode(Path.GetFileName(path), children, path, Path.GetRelativePath(srcPath, path));
    }
}