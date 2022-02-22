using System.IO;
using System.Threading.Tasks;

namespace ScriptBee.ProjectContext;

public interface IProjectFileStructureManager
{
    public void CreateProjectFolderStructure(string projectId);

    public void CreateFile(string projectId, string relativePath, string fileContent);

    public bool FileExists(string projectId, string relativePath);

    public string GetFileContent(string projectId, string relativePath);

    public Task<string> GetFileContentAsync(string projectId, string relativePath);

    public FileStream GetFileContentStream(string projectId, string relativePath);

    public FileTreeNode GetSrcStructure(string projectId);
    
    public string GetAbsoluteFilePath(string projectId, string filePath);
}