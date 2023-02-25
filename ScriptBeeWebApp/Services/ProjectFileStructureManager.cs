using Microsoft.Extensions.Options;
using ScriptBee.Config;
using ScriptBee.ProjectContext;
using ScriptBee.Services.Config;
using ScriptBeeWebApp.Data.Exceptions;

namespace ScriptBeeWebApp.Services;

// todo add tests
public class ProjectFileStructureManager : IProjectFileStructureManager
{
    private readonly IFileWatcherService _fileWatcherService;
    private readonly string _userFolderPath;

    public ProjectFileStructureManager(IOptions<UserFolderSettings> userFolderSettings,
        IFileWatcherService fileWatcherService)
    {
        _userFolderPath = userFolderSettings.Value.UserFolderPath ?? "";
        _fileWatcherService = fileWatcherService;
    }

    public void CreateProjectFolderStructure(string projectId)
    {
        var projectPath = Path.Combine(ConfigFolders.PathToProjects, projectId);
        Directory.CreateDirectory(projectPath);
        Directory.CreateDirectory(Path.Combine(projectPath, ConfigFolders.SrcFolder));
        Directory.CreateDirectory(Path.Combine(projectPath, ConfigFolders.GeneratedFolder));
    }

    public void DeleteProjectFolderStructure(string projectId)
    {
        var projectPath = Path.Combine(ConfigFolders.PathToProjects, projectId);
        Directory.Delete(projectPath, true);
    }

    public void CreateFile(string projectId, string relativePath, string fileContent)
    {
        var filePath = Path.Combine(ConfigFolders.PathToProjects, projectId, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, fileContent);
    }

    public FileTreeNode CreateSrcFile(string projectId, string relativePath, string fileContent)
    {
        var filePath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, fileContent);

        var srcPath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder);

        return new FileTreeNode(Path.GetFileName(relativePath), filePath,
            Path.GetRelativePath(srcPath, filePath), null);
    }

    public bool FileExists(string projectId, string relativePath)
    {
        var filePath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder, relativePath);
        return File.Exists(filePath);
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
        return GetPathToUserFolder(absolutePath);
    }

    public void DeleteFolder(string projectId, string pathToFolder)
    {
        var folderAbsolutePath = Path.Combine(ConfigFolders.PathToProjects, projectId, pathToFolder);

        if (Directory.Exists(folderAbsolutePath))
        {
            Directory.Delete(folderAbsolutePath, true);
        }
    }

    public string GetProjectAbsolutePath(string projectId)
    {
        var projectPath = Path.Combine(ConfigFolders.PathToProjects, projectId);
        return GetPathToUserFolder(projectPath);
    }

    public void SetupFileWatcher(string projectId)
    {
        var fullPath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder);

        if (!Directory.Exists(fullPath))
        {
            throw new ProjectFolderNotFoundException(projectId);
        }

        _fileWatcherService.SetupFileWatcher(fullPath);
    }

    public void RemoveFileWatcher(string projectId)
    {
        var fullPath = Path.Combine(ConfigFolders.PathToProjects, projectId, ConfigFolders.SrcFolder);

        if (!Directory.Exists(fullPath))
        {
            throw new ProjectFolderNotFoundException(projectId);
        }

        _fileWatcherService.RemoveFileWatcher(fullPath);
    }

    private FileTreeNode GetFolderStructure(string path, string srcPath)
    {
        if (File.Exists(path))
        {
            return new FileTreeNode(Path.GetFileName(path), path, Path.GetRelativePath(srcPath, path), null);
        }

        var children = new List<FileTreeNode>();

        foreach (var folderPath in Directory.GetDirectories(path))
        {
            children.Add(GetFolderStructure(folderPath, srcPath));
        }

        foreach (var filePath in Directory.GetFiles(path))
        {
            children.Add(new FileTreeNode(Path.GetFileName(filePath), filePath,
                Path.GetRelativePath(srcPath, filePath), null));
        }

        return new FileTreeNode(Path.GetFileName(path), path, Path.GetRelativePath(srcPath, path), children);
    }

    private string GetPathToUserFolder(string absolutePath)
    {
        if (string.IsNullOrEmpty(_userFolderPath))
        {
            return absolutePath;
        }

        var part = absolutePath.Replace("\\", "/")
            .Replace(ConfigFolders.PathToRoot.Replace("\\", "/"), "");

        return Path.Combine(_userFolderPath, part.TrimStart('\\', '/')).Replace("\\", "/");
    }
}
