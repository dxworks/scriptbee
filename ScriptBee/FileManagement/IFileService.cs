using System.Collections.Generic;

namespace ScriptBee.FileManagement;

public interface IFileService
{
    string GetFileName(string path);
    
    IEnumerable<string> GetDirectories(string path);

    bool FileExists(string path);
    
    string CombinePaths(string path1, string path2);

    void DeleteFile(string path);

    void DeleteFolder(string path);

    string ReadAllText(string path);

    void AppendTextToFile(string path, string text);
}
