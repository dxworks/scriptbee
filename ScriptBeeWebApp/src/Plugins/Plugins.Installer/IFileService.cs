namespace ScriptBee.Plugins.Installer;

public interface IFileService
{
    bool DirectoryExists(string path);

    bool FileExists(string path);

    string CombinePaths(string path1, string path2);

    void DeleteFile(string path);

    void DeleteDirectory(string path);

    IEnumerable<string> ReadAllLines(string path);

    void AppendTextToFile(string path, string text);

    void CreateFolder(string path);
}
