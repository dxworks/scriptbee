using System.IO;
using System.Threading.Tasks;

namespace ScriptBee.ProjectContext;

public interface IProjectFileStructureManager
{
    public void CreateProjectFolderStructure(string projectId);

    public void DeleteProjectFolderStructure(string projectId);

    public void CreateFile(string projectId, string relativePath, string fileContent);

    public FileTreeNode CreateSrcFile(string projectId, string relativePath, string fileContent);

    public bool FileExists(string projectId, string relativePath);

    public Task<string?> GetFileContentAsync(string projectId, string relativePath);

    public FileStream GetFileContentStream(string projectId, string relativePath);

    public FileTreeNode? GetSrcStructure(string projectId);

    public string GetAbsoluteFilePath(string projectId, string filePath);

    public void DeleteFolder(string projectId, string pathToFolder);

    public void DeleteFile(string projectId, string pathToFile);

    public string GetProjectAbsolutePath(string projectId);
    void SetupFileWatcher(string projectId);
    void RemoveFileWatcher(string projectId);
}
