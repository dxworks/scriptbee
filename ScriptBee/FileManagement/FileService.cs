using System.Collections.Generic;
using System.IO;

namespace ScriptBee.FileManagement;

public sealed class FileService : IFileService
{
    public string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }

    public IEnumerable<string> GetDirectories(string path)
    {
        try
        {
            return Directory.GetDirectories(path);
        }
        catch
        {
            return new List<string>();
        }
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public string CombinePaths(string path1, string path2)
    {
        return Path.Combine(path1, path2);
    }

    public void DeleteFile(string path)
    {
        File.Delete(path);
    }

    public void DeleteFolder(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }

    public void AppendTextToFile(string path, string text)
    {
        File.AppendAllLines(path, new List<string> { text });
    }
}
