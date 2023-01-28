using System.Collections.Generic;
using System.IO;

namespace ScriptBee.FileManagement;

public class FileService : IFileService
{
    public IEnumerable<string> GetDirectories(string path)
    {
        return Directory.GetDirectories(path);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public string CombinePaths(string path1, string path2)
    {
        return Path.Combine(path1, path2);
    }
}
