namespace ScriptBee.Plugins.Installer;

public sealed class FileService : IFileService
{
    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
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
        if (FileExists(path))
        {
            File.Delete(path);
        }
    }

    public void DeleteDirectory(string path)
    {
        if (DirectoryExists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public IEnumerable<string> ReadAllLines(string path)
    {
        return File.ReadAllLines(path);
    }

    public void AppendTextToFile(string path, string text)
    {
        File.AppendAllLines(path, new List<string> { text });
    }

    public void CreateFolder(string path)
    {
        Directory.CreateDirectory(path);
    }
}
